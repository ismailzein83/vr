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


        [HttpGet]
        public CommonEnums.OperatorType GetOperatorType()
        {
            return ConfigParameterManager.GetOperatorType();
        }

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
        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdate input)
        {
            CaseManagmentManager manager = new CaseManagmentManager();
            return manager.UpdateAccountCase(input);
        }
        
    }
}