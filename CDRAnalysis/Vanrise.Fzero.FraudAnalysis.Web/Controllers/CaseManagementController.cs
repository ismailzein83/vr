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

        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase> SaveAccountCase(AccountCase accountCaseObject)
        {
            CaseManagmentManager manager = new CaseManagmentManager();

            return manager.SaveAccountCase(accountCaseObject);
        }


        //[HttpPost]
        //public object GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        //{

        //    CaseManagmentManager manager = new CaseManagmentManager();

        //    UserManager userManager = new UserManager();

        //    return GetWebResponse(input, manager.GetFilteredAccountCases(input, userManager.GetUsers()));
        //}


        [HttpPost]
        public object GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            FraudManager manager = new FraudManager();
            return GetWebResponse(input, manager.GetFilteredSuspiciousNumbers(input));
        }

        [HttpPost]
        public object GetFilteredAccountSuspicionSummaries(Vanrise.Entities.DataRetrievalInput<AccountSuspicionSummaryQuery> input)
        {
            FraudManager manager = new FraudManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionSummaries(input));
        }

        [HttpPost]
        public object GetFilteredAccountSuspicionDetails(Vanrise.Entities.DataRetrievalInput<AccountSuspicionDetailQuery> input)
        {
            FraudManager manager = new FraudManager();
            return GetWebResponse(input, manager.GetFilteredAccountSuspicionDetails(input));
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<AccountSuspicionSummary> UpdateAccountCase(AccountCaseUpdate input)
        {
            FraudManager manager = new FraudManager();
            return manager.UpdateAccountCase(input);
        }

        [HttpGet]
        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, string strategiesList, string suspicionLevelsList, string accountNumber)
        {
            FraudManager manager = new FraudManager();

            List<int> strategiesIntList = new List<int>();
            if (strategiesList != null)
                strategiesIntList = strategiesList.Split(',').Select(h => int.Parse(h)).ToList();



            List<int> suspicionLevelsIntList = new List<int>();
            if (suspicionLevelsList != null)
                suspicionLevelsIntList = suspicionLevelsList.Split(',').Select(h => int.Parse(h)).ToList();



            return manager.GetFraudResult(fromDate, toDate, strategiesIntList, suspicionLevelsIntList, accountNumber);
        }

        [HttpGet]
        public CommonEnums.OperatorType GetOperatorType()
        {
            return ConfigParameterManager.GetOperatorType();
        }



    }
}