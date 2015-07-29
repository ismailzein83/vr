using System.Web.Http;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace  Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    public class CaseManagementController : BaseAPIController
    {

        [HttpPost]
        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<SubscriberCase> SaveSubscriberCase(SubscriberCase subscriberCaseObject)
        {
            CaseManagmentManager manager = new CaseManagmentManager();

            return manager.SaveSubscriberCase(subscriberCaseObject);
        }



    }
}