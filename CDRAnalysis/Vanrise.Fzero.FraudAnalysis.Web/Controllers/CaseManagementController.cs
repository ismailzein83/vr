using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class CaseManagementController : BaseAPIController
    {
        //[HttpGet]
        //public OperatorTypeEnum GetOperatorType()
        //{
        //    return ConfigParameterManager.GetOperatorType();
        //}

        [HttpPost]
        public object GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionSummaries(input));
        }

        [HttpPost]
        public object GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionDetails(input));
        }

        [HttpPost]
        public object GetFilteredCasesByAccountNumber(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredCasesByAccountNumber(input));
        }

        [HttpPost]
        public object GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return GetWebResponse(input, manager.GetFilteredDetailsByCaseID(input));
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdateResultQuery input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.UpdateAccountCase(input);
        }
    }
}