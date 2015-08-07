﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise.Web
{
    public class LicenseCheckFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        #region Local Licensed Variables
        private static bool? s_isLicensed;//Tone Lisenced value
        private static DateTime? s_licensedActivatationDate;//TOne Licensed Date
        #endregion

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            LicenseCheker();
            base.OnAuthorization(actionContext);
        }

        private static void LicenseCheker()
        {
            string licenseKey = string.Empty;
            if (s_licensedActivatationDate.HasValue && (DateTime.Now - s_licensedActivatationDate.Value).TotalHours >= 6.0)
                s_isLicensed = null;
            if (!s_isLicensed.HasValue)
            {
                s_isLicensed = Vanrise.Components.Security.LicenceManagerControl.CheckLicence("Vanrise License", out licenseKey);
                if (s_isLicensed.Value) s_licensedActivatationDate = DateTime.Now;
            }
            if (!s_isLicensed.Value)
                throw new Exception(string.Format("License Key is expired. key is:{0}", licenseKey));
        }


    }
}