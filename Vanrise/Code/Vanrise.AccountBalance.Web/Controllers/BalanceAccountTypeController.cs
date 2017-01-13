using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
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
        public object GetBalanceAccountTypeInfos(string filter = null)
        {
            AccountTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<AccountTypeInfoFilter>(filter) : null;
            AccountTypeManager manager = new AccountTypeManager();
            return manager.GetAccountTypeInfo(deserializedFilter);
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