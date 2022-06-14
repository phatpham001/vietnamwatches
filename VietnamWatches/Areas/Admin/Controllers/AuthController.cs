using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyClass.Models;
using MyClass.DAO;

namespace VietnamWatches.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        UserDAO userDAO = new UserDAO();
        // GET: Admin/Auth
        public ActionResult Login()
        {
            if (!Session["UserAdmin"].Equals(""))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.ErrorLogin = "";
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection field)
        {
            string strError = "";
            string username = field["username"];
            string password = XString.ToMD5(field["password"]);
            User user = userDAO.getRowAdmin(username);

            if (user==null)
            {
                strError = "Tên đăng nhập không tồn tại";
            }
            else 
            {
                if (user.Password.Equals(password))
                {
                    Session["UserId"] = user.Id;
                    Session["UserAdmin"] = user.UserName;
                    Session["FullName"] = user.FullName;
                    return RedirectToAction("Index", "Dashboard");
                }  
                else
                {
                    strError = "Mật khẩu không đúng";
                }
            }
            ViewBag.ErrorLogin = "<span class='text-danger'>" + strError + "</span>";
            return View();
        }
        public ActionResult Logout()
        {
            Session["UserId"] = "";
            Session["UserAdmin"] = "";
            Session["FullName"] = "";
            return RedirectToAction("Login", "Auth");

        }
    }
}