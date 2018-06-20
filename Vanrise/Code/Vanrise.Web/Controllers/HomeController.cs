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
        VRLocalizationManager _vrLocalizationManager = new VRLocalizationManager();
        public ActionResult Index()
        {

            ViewBag.Title = "Home Page";
            ViewBag.CookieName = (new SecurityManager()).GetCookieName();
            GeneralSettingsManager settingManager = new GeneralSettingsManager();
            var cacheSettingData = settingManager.GetCacheSettingData();
            ViewBag.version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
            Common.Business.ConfigManager cManager = new Common.Business.ConfigManager();
            ViewBag.ProductVersion = cManager.GetProductVersionNumber();
            ViewBag.CompanyName = cManager.GetDefaultCompanyName();
            ViewBag.isEnabledGA = settingManager.GetGoogleAnalyticsEnabled();

            var isLocalizationEnabled = new VRLocalizationManager().IsLocalizationEnabled();
            ViewBag.IsLocalizationEnabled = isLocalizationEnabled.ToString().ToLower();
            var isRTL = _vrLocalizationManager.IsRTL();
            ViewBag.IsRTL = isRTL.ToString().ToLower();
            ViewBag.RTLClass = isRTL ? " class= rtl " : "";


            Vanrise.Security.Business.SecurityManager securityManager = new SecurityManager();
            var loginUrl = securityManager.GetLoginURL();
            if (loginUrl != null)
                ViewBag.LoginURL = loginUrl;
            int? loggedInUserId;
            if (SecurityContext.Current.TryGetLoggedInUserId(out loggedInUserId))
                ViewBag.HasRemoteApplications = new Vanrise.Security.Business.RegisteredApplicationManager().HasRemoteApplications(loggedInUserId.Value).ToString().ToLower();

            return View("~/Client/CSViews/Home/Index.cshtml");
        }
    }
}
