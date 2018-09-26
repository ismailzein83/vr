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
        AccountTypeManager _accountTypeManager = new AccountTypeManager();

        [HttpGet]
        [Route("GetCurrentAccountBalance")]
        public CurrentAccountBalance GetCurrentAccountBalance(String accountId, Guid accountTypeId)
        {
            LiveBalanceManager manager = new LiveBalanceManager();
            return manager.GetCurrentAccountBalance(accountTypeId, accountId);
        }

        [HttpPost]
        [Route("GetFilteredAccountBalances")]
        public object GetFilteredAccountBalances(Vanrise.Entities.DataRetrievalInput<AccountBalanceQuery> input)
        {
            if (!_accountTypeManager.DoesUserHaveViewAccess(input.Query.AccountTypeId))
                return GetUnauthorizedResponse();
            LiveBalanceManager manager = new LiveBalanceManager();
            return GetWebResponse(input, manager.GetFilteredAccountBalances(input), "Account Live Balances");
        }

    }
}