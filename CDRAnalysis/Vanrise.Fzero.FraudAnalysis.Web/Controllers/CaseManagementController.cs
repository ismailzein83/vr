using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;
using Vanrise.Security.Business;

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



    }
}