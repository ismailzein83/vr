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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BankDetail")]
    public class VRCommon_BankDetailController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBankDetailsInfo")]
        public IEnumerable<BankDetailsSettingsInfo> GetBankDetailsInfo()
        {
            ConfigManager manager = new ConfigManager();
            return manager.GetBankDetailsInfo();
        }

    }
}