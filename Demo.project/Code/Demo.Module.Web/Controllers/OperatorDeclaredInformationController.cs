using Demo.Module.Business;
using Demo.Module.Entities;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorDeclaredInformation")]
    public class Demo_OperatorDeclaredInformationController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredOperatorDeclaredInformations")]
         public object GetFilteredOperatorDeclaredInformations(Vanrise.Entities.DataRetrievalInput<OperatorDeclaredInformationQuery> input)
        {
            OperatorDeclaredInformationManager manager = new OperatorDeclaredInformationManager();
            return GetWebResponse(input, manager.GetFilteredOperatorDeclaredInformations(input));
        }

        [HttpGet]
        [Route("GetOperatorDeclaredInformation")]
        public OperatorDeclaredInformation GetOperatorDeclaredInformation(int OperatorDeclaredInformationId)
        {
            OperatorDeclaredInformationManager manager = new OperatorDeclaredInformationManager();
            return manager.GetOperatorDeclaredInformation(OperatorDeclaredInformationId);
        }

        [HttpPost]
        [Route("AddOperatorDeclaredInformation")]
        public Vanrise.Entities.InsertOperationOutput<OperatorDeclaredInformationDetail> AddOperatorDeclaredInformation(OperatorDeclaredInformation operatorDeclaredInformation)
        {
            OperatorDeclaredInformationManager manager = new OperatorDeclaredInformationManager();
            return manager.AddOperatorDeclaredInformation(operatorDeclaredInformation);
        }
        [HttpPost]
        [Route("UpdateOperatorDeclaredInformation")]
        public Vanrise.Entities.UpdateOperationOutput<OperatorDeclaredInformationDetail> UpdateOperatorDeclaredInformation(OperatorDeclaredInformation operatorDeclaredInformation)
        {
            OperatorDeclaredInformationManager manager = new OperatorDeclaredInformationManager();
            return manager.UpdateOperatorDeclaredInformation(operatorDeclaredInformation);
        }
    }
}