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

            var isLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();
            if (isLocalizationEnabled)
            {
                Guid? languageId = LocalizationConfig.GetLanguageIdFromCookies(Request.Cookies);
                if (languageId.HasValue)
                {
                    var isRTL = new VRLocalizationLanguageManager().IsRTL(languageId.Value);
                    ViewBag.IsRTL = isRTL.ToString().ToLower();
                    ViewBag.RTLClass = isRTL ? " class= rtl " : "";
                }
            }


            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Login.cshtml");
        }

        public ActionResult Payment(string redirectTo)
        {
            ViewBag.Title = "Payment";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            var isLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();
            if (isLocalizationEnabled)
            {
                Guid? languageId = LocalizationConfig.GetLanguageIdFromCookies(Request.Cookies);
                if (languageId.HasValue)
                {
                    var isRTL = new VRLocalizationLanguageManager().IsRTL(languageId.Value);
                    ViewBag.IsRTL = isRTL.ToString().ToLower();
                    ViewBag.RTLClass = isRTL ? " class= rtl " : "";
                }
            }

            ViewBag.RedirectTo = redirectTo;
            return View("~/Client/CSViews/Security/Payment.cshtml");
        }
    }
}