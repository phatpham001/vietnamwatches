using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Models;

namespace VietnamWatches.Controllers
{
    public class GioHangController : Controller
    {
        ProductDAO productDAO = new ProductDAO();
        UserDAO userDAO = new UserDAO();
        OrderDAO orderDAO = new OrderDAO();
        OrderDetailDAO orderdetailDAO = new OrderDetailDAO();
        XCart xCart = new XCart();
        // GET: Cart
        public ActionResult Index()
        {
            //Lấy ra giỏ hàng
            List<CartItem> listCart = xCart.getCart();
            return View("Index", listCart);
        }
        public ActionResult AddCart(int productId)
        {
            Product product = productDAO.getRow(productId); //Lấy ra chi tiết sản phẩm
            CartItem cartItem = new CartItem(product.Id, product.Name, product.Images, product.PriceSale, 1);

            //Thêm vào giỏ hàng
            List<CartItem> listCart = xCart.AddCart(cartItem, productId);

            return Json(listCart);
        }
        public ActionResult AddCartDetail(int productId, int qty)
        {
            Product product = productDAO.getRow(productId); //Lấy ra chi tiết sản phẩm
            CartItem cartItem = new CartItem(product.Id, product.Name, product.Images, product.PriceSale, qty);

            //Thêm vào giỏ hàng
            List<CartItem> listCart = xCart.AddCartDetail(cartItem, productId, qty);

            return Json(listCart);
        }
        public ActionResult DelCart(int productId)
        {
            xCart.DeleteCart(productId);
            return RedirectToAction("Index", "GioHang");
        }
        [HttpPost]
        public JsonResult UpdateCart(int qty, int vitri)
        {
            //string strError = "";
            //if (!string.IsNullOrEmpty(form["clickUpdate"]))
            //{
            //    var listQty = form["qty"];
            //    var listArr = listQty.Split(',');
            //    xCart.UpdateCart(listArr);
            //}
            //strError = vitri.ToString() + qty.ToString();

            //var listQty = qty;
            List<CartItem> listCart = xCart.getCart();
            int vt = vitri;
            Product product = productDAO.getRow(listCart[vt].ProductId);

            if (product.Number <= listCart[vt].QtyBuy)
            {
                return Json(new { Success = false, Cart = listCart }, JsonRequestBehavior.AllowGet);
            }

            xCart.UpdateCart(qty, vitri);
            //update xong tra ve cart 
            listCart = (List<CartItem>)System.Web.HttpContext.Current.Session["MyCart"];
            //tim cart roi tra ve json
            return Json(new { Success = true, Cart = listCart }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelAllCart()
        {
            xCart.DelAllCart();
            return RedirectToAction("Index", "GioHang");

        }

        //Thanh toán đơn hàng
        public ActionResult ThanhToan()
        {
            //Lấy ra giỏ hàng
            List<CartItem> listCart = xCart.getCart();
            //Kiểm tra đăng nhập trang người dùng
            if (Session["UserCustomer"].Equals(""))
            {
                return Redirect("~/login");//Chuyển đến trang đăng nhập
            }

            int userId = int.Parse(Session["CustomerId"].ToString()); //Lấy ra thông tin đăng nhập
            User user = userDAO.getRow(userId);
            ViewBag.User = user;

            return View("ThanhToan", listCart);

        }
        //Đặt mua đơn hàng
        [HttpPost]
        public ActionResult DatMua(FormCollection field)
        {
            if (Session["CustomerId"].Equals(""))
            {
                return Redirect("~/login");//Chuyển đến trang đăng nhập
            }
            string email = "";
            string name = "";
            string address = "";
            string phone = "";
            string notemail = "";
            //Lưu thông tin vào csdl
            int userId = int.Parse(Session["CustomerId"].ToString()); //Lấy ra thông tin đăng nhập
            User user = userDAO.getRow(userId);
            Order order = new Order();

            if (field["deliveryName"].Equals(""))
            {
                //Lấy thông tin
                string note = field["deliveryNote"];
                //Tạo đối tượng đơn hàng
                order.DeliveryNote = note;
                //-----------
                name = user.UserName;
                email = user.Email;
                address = user.Address;
                phone = user.Phone;
                notemail = note;
            }
            else
            {
                string deliveryName = field["deliveryName"];
                string deliveryEmail = field["deliveryEmail"];
                string deliveryAddress = field["deliveryAddress"];
                string deliveryPhone = field["deliveryPhone"];
                string note = field["deliveryNote"];
                //Tạo đối tượng đơn hàng
                order.DeliveryName = deliveryName;
                order.DeliveryEmail = deliveryEmail;
                order.DeliveryAddress = deliveryAddress;
                order.DeliveryPhone = deliveryPhone;
                order.DeliveryNote = note;
                //-----------
                name = deliveryName;
                email = deliveryEmail;
                address = deliveryAddress;
                phone = deliveryPhone;
                notemail = note;

            }
            order.UserId = userId;
            order.Status = 1;

            var chars = "0123456789";
            var stringChars = new char[15];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] += chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);

            order.Code = finalString;
            order.UpdatedBy = (Session["CustomerId"].Equals("")) ? 1 : int.Parse(Session["CustomerId"].ToString());
            order.UpdatedAt = DateTime.Now;
            order.ExportDate = DateTime.Now;
            order.CreateDate = DateTime.Now;

            if (orderDAO.Insert(order) == 1)
            {
                //Thêm vào chi tiết đơn hàng
                List<CartItem> listCart = xCart.getCart();
                foreach (CartItem cartItem in listCart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.Id;
                    orderDetail.ProductId = cartItem.ProductId;
                    orderDetail.Price = cartItem.PriceBuy;
                    orderDetail.Amount = cartItem.Amount;

                    orderDetail.Quantity = cartItem.QtyBuy;
                    Product product = productDAO.getRow(orderDetail.ProductId);
                    product.Number = product.Number - cartItem.QtyBuy;
                    productDAO.Update(product);

                    orderdetailDAO.Insert(orderDetail);
                }


                MailMessage mail = new MailMessage();
                // you need to enter your mail address
                mail.From = new MailAddress("vietnamwatches.info@gmail.com");

                //To Email Address - your need to enter your to email address
                mail.To.Add(email);

                mail.Subject = "Đơn hàng mới từ VietnamWatches";

                //you can specify also CC and BCC - i will skip this
                //mail.CC.Add("");
                //mail.Bcc.Add("");

                mail.IsBodyHtml = true;

                string content = "Name : " + name;
                content += "<br/> Message : " + notemail;



                mail.Body = content;


                //create SMTP instant

                //you need to pass mail server address and you can also specify the port number if you required
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");//google hay gmail ?

                //Create nerwork credential and you need to give from email address and password
                NetworkCredential networkCredential = new NetworkCredential("vietnamwatches.info@gmail.com", "wqqsugcuqcijsfaj");
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredential;
                smtpClient.Port = 587; // this is default port number - you can also change this
                smtpClient.EnableSsl = true; // if ssl required you need to enable it
                smtpClient.Send(mail);

            }
            ViewBag.OrderId = order.Id;
            TempData["mydata"] = order.Id;
            xCart.DeleteCart();
            return Redirect("~/dat-hang-thanh-cong");//Chuyển đến trang đăng nhập
        }
    }
}


