using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace Vanrise.Web.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult Login(string redirectTo)
        {
            ViewBag.Title = "Login Page";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            ViewBag.IsLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled().ToString().ToLower();

            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Login.cshtml");
        }

        public ActionResult Payment(string redirectTo)
        {
            ViewBag.Title = "Payment";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            ViewBag.IsLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled().ToString().ToLower();
            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Payment.cshtml");
        }
    }
}