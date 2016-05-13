using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vanrise.Security.Business;

namespace Vanrise.Web.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult Login(string redirectTo)
        {
            ViewBag.Title = "Login Page";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Login.cshtml");
        }

        public ActionResult Payment(string redirectTo)
        {
            ViewBag.Title = "Payment";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Payment.cshtml");
        }
    }
}