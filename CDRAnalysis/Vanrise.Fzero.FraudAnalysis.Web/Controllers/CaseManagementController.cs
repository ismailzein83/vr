using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class CaseManagementController : BaseAPIController
    {
        

        [HttpPost]
        public object GetFilteredAccountSuspicionSummaries(DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionSummaries(input));
        }

        [HttpPost]
        public object GetFilteredAccountSuspicionDetails(DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionDetails(input));
        }

        [HttpPost]
        public object GetFilteredCasesByAccountNumber(DataRetrievalInput<AccountCaseQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredCasesByAccountNumber(input));
        }



        [HttpPost]
        public object GetFilteredCasesByFilters(DataRetrievalInput<CancelAccountCasesQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredCasesByFilters(input));
        }

        [HttpPost]
        public object GetFilteredDetailsByCaseID(DataRetrievalInput<CaseDetailQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredDetailsByCaseID(input));
        }

        [HttpGet]
        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.GetRelatedNumbersByAccountNumber(accountNumber);
        }

        [HttpGet]
        public AccountCase GetAccountCase(int caseID)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.GetAccountCase(caseID);
        }

        [HttpPost]
        public object GetFilteredAccountCaseHistoryByCaseID(DataRetrievalInput<AccountCaseLogQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredAccountCaseHistoryByCaseID(input));
        }

        [HttpPost]
        public UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateQuery input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.UpdateAccountCase(input);
        }

        [HttpPost]
        public UpdateOperationOutput<AccountCase> CancelAccountCases(CancelAccountCasesQuery input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.CancelAccountCases(input);
        }

        [HttpPost]
        public UpdateOperationOutput<AccountCase> CancelSelectedAccountCases(List<int> caseIds)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.CancelSelectedAccountCases(caseIds);
        }



        
    }
}