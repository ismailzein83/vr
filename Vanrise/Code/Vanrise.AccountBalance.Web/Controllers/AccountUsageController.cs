using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountUsage")]
    [JSONWithTypeAttribute]
    public class AccountUsageController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountUsagePeriodSettingsConfigs")]
        public object GetAccountUsagePeriodSettingsConfigs()
        {
            AccountUsageManager manager = new AccountUsageManager();
            return manager.GetAccountUsagePeriodSettingsConfigs();
        }
    }
}