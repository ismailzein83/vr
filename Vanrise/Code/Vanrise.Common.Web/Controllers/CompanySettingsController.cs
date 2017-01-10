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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CompanySettings")]
    public class VRCommon_CompanySettingsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCompanySettingsInfo")]
        public IEnumerable<CompanySettingsInfo> GetCompanySettingsInfo()
        {
            ConfigManager manager = new ConfigManager();
            return manager.GetCompanySettingsInfo();
        }
    }
}