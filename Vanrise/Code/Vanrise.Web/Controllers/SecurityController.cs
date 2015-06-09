using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vanrise.Web.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult Login()
        {
            ViewBag.Title = "Login Page";

            return View("~/Client/CSViews/Security/Login.cshtml");
        }
    }
}