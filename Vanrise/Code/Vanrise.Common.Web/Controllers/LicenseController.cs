using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "License")]
    [JSONWithTypeAttribute]
    public class LicenseController : BaseAPIController
    {
        [HttpGet]
        [Route("GetLicenseExpiryDate")]
        public DateTime GetLicenseExpiryDate()
        {
            LicenseManager manager = new LicenseManager();
            return manager.GetLicenseExpiryDate();
        }
    }
}
