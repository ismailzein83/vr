using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace Vanrise.Web.Controllers
{

    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
          
            ViewBag.Title = "Home Page";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            GeneralSettingsManager settingManager =  new GeneralSettingsManager();
            var cacheSettingData = settingManager.GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            Common.Business.ConfigManager cManager = new Common.Business.ConfigManager();
            ViewBag.ProductVersion = cManager.GetProductVersionNumber();
            ViewBag.CompanyName = cManager.GetDefaultCompanyName();
            ViewBag.isEnabledGA = settingManager.GetGoogleAnalyticsEnabled();

            var isLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();
            if(isLocalizationEnabled)
            {
                Guid? languageId = LocalizationConfig.GetLanguageIdFromCookies(Request.Cookies);
                if (languageId.HasValue)
                {
                    var isRTL = new VRLocalizationLanguageManager().IsRTL(languageId.Value);
                    ViewBag.IsRTL = isRTL.ToString().ToLower();
                    ViewBag.RTLClass = isRTL ? " class= rtl " : "";
                }
            }
          
           
            Vanrise.Security.Business.SecurityManager securityManager = new SecurityManager();
            var loginUrl = securityManager.GetLoginURL();
            if (loginUrl != null)
                ViewBag.LoginURL = loginUrl;
            return View("~/Client/CSViews/Home/Index.cshtml");
        }
    }
}
