using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountCase")]
    [JSONWithTypeAttribute]
    public class AccountCaseController : BaseAPIController
    {
        [HttpGet]
        [Route("GetLastAccountCase")]
        public AccountCase GetLastAccountCase(string accountNumber)
        {
            AccountCaseManager manager = new AccountCaseManager();
            return manager.GetLastAccountCase(accountNumber);
        }

        [HttpPost]
        [Route("GetFilteredAccountSuspicionSummaries")]
        public object GetFilteredAccountSuspicionSummaries(DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            AccountCaseManager manager = new AccountCaseManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionSummaries(input));
        }

        [HttpPost]
        [Route("UpdateAccountCase")]
        public UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateQuery input)
        {
            AccountCaseManager manager = new AccountCaseManager();
            return manager.UpdateAccountCase(input);
        }

        [HttpGet]
        [Route("GetAccountCase")]
        public AccountCase GetAccountCase(int caseID)
        {
            AccountCaseManager manager = new AccountCaseManager();
            return manager.GetAccountCase(caseID);
        }

        [HttpPost]
        [Route("GetFilteredCasesByAccountNumber")]
        public object GetFilteredCasesByAccountNumber(DataRetrievalInput<AccountCaseQuery> input)
        {
            AccountCaseManager manager = new AccountCaseManager();
            return GetWebResponse(input, manager.GetFilteredCasesByAccountNumber(input));
        }

    }
}