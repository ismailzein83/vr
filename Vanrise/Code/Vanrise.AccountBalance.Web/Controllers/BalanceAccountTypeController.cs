using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BalanceAccountType")]
    [JSONWithTypeAttribute]
    public class BalanceAccountTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBalanceAccountTypeInfos")]
        public object GetBalanceAccountTypeInfos()
        {
            AccountTypeManager manager = new AccountTypeManager();
            return manager.GetAccountTypeInfo();
        }
        [HttpGet]
        [Route("GetAccountBalanceExtendedSettingsConfigs")]
        public object GetAccountBalanceExtendedSettingsConfigs()
        {
            AccountTypeManager manager = new AccountTypeManager();
            return manager.GetAccountBalanceExtendedSettingsConfigs();
        }
    }
}