using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VietnamWatches
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Session_Start()
        {
            //L?u th�ng tin --> Trang qu?n tr?
            Session["UserId"] = "";
            Session["UserAdmin"] = "";
            Session["FullName"] = "";
            //Session["ImageAdmin"] = "";
            Session["MyCart"] = "";

            //L?u th�ng tin --> Trang ng??i d�ng
            Session["CustomerId"] = "";
            Session["UserCustomer"] = "";
            Session["FullNameCustomer"] = "";
        }

    }
}
