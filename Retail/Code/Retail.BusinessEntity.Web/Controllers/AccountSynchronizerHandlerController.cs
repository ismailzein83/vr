using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountSynchronizerHandler")]
    public class AccountSynchronizerHandlerController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountSynchronizerInsertHandlerConfigs")]
        public IEnumerable<AccountSynchronizerInsertHandlerConfig> GetAccountSynchronizerInsertHandlerConfigs()
        {
            AccountSynchronizerHandlerManager manager = new AccountSynchronizerHandlerManager();
            return manager.GetAccountSynchronizerInsertHandlerConfigs();
        }
    }
}