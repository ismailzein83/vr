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
    [RoutePrefix(Constants.ROUTE_PREFIX + "LiveBalance")]
    public class LiveBalanceController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCurrentAccountBalance")]
        public CurrentAccountBalance GetCurrentAccountBalance(long accountId, Guid accountTypeId)
        {
            LiveBalanceManager manager = new LiveBalanceManager();
            return manager.GetCurrentAccountBalance(accountTypeId, accountId);
        }
    }
}