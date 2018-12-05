using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace Vanrise.Web.Base.Controllers
{
    public class SecurityController : Controller
    {
        VRLocalizationManager _vrLocalizationManager = new VRLocalizationManager();

        public ActionResult Login(string redirectTo)
        {
            ViewBag.Title = "Login Page";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;

            var isLocalizationEnabled = _vrLocalizationManager.IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();
            var isRTL = _vrLocalizationManager.IsRTL();
            ViewBag.IsRTL = isRTL.ToString().ToLower();
            ViewBag.RTLClass = isRTL ? " class= rtl " : "";

            ViewBag.RedirectTo = redirectTo;
            return View("/Client/CSViews/Security/Login.cshtml");
        }

        public ActionResult Payment(string redirectTo)
        {
            ViewBag.Title = "Payment";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            var isLocalizationEnabled = _vrLocalizationManager.IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();

            var isRTL = _vrLocalizationManager.IsRTL();
            ViewBag.IsRTL = isRTL.ToString().ToLower();
            ViewBag.RTLClass = isRTL ? " class= rtl " : "";

            ViewBag.RedirectTo = redirectTo;
            return View("/Client/CSViews/Security/Payment.cshtml");
        }
    }
}
