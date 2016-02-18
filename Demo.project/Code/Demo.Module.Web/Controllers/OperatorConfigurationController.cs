using Demo.Module.Business;
using Demo.Module.Entities;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorConfiguration")]
    public class Demo_OperatorConfigurationController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredOperatorConfigurations")]
        public object GetFilteredOperatorConfigurations(Vanrise.Entities.DataRetrievalInput<OperatorConfigurationQuery> input)
        {
            OperatorConfigurationManager manager = new OperatorConfigurationManager();
            return GetWebResponse(input, manager.GetFilteredOperatorConfigurations(input));
        }

        [HttpGet]
        [Route("GetOperatorConfiguration")]
        public OperatorConfiguration GetOperatorConfiguration(int OperatorConfigurationId)
        {
            OperatorConfigurationManager manager = new OperatorConfigurationManager();
            return manager.GetOperatorConfiguration(OperatorConfigurationId);
        }

        [HttpPost]
        [Route("AddOperatorConfiguration")]
        public Vanrise.Entities.InsertOperationOutput<OperatorConfigurationDetail> AddOperatorConfiguration(OperatorConfiguration operatorConfiguration)
        {
            OperatorConfigurationManager manager = new OperatorConfigurationManager();
            return manager.AddOperatorConfiguration(operatorConfiguration);
        }
        [HttpPost]
        [Route("UpdateOperatorConfiguration")]
        public Vanrise.Entities.UpdateOperationOutput<OperatorConfigurationDetail> UpdateOperatorConfiguration(OperatorConfiguration operatorConfiguration)
        {
            OperatorConfigurationManager manager = new OperatorConfigurationManager();
            return manager.UpdateOperatorConfiguration(operatorConfiguration);
        }
    }
}