using System.Web.Http;
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


        [HttpPost]
        public object GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input)
        {

            UserController userManager = new UserController();

            CaseManagmentManager manager = new CaseManagmentManager();

            return GetWebResponse(input, manager.GetFilteredAccountCases(input, userManager.GetUsers()));
        }



    }
}