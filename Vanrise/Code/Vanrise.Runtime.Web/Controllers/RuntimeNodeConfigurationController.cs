using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Entities.Configuration;
using Vanrise.Web.Base;
using Vanrise.Runtime;
using Vanrise.Runtime.Business;
using Vanrise.Entities;

namespace Vanrise.Runtime.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RuntimeNodeConfiguration")]
    [JSONWithTypeAttribute]
    public class VRRuntime_RuntimeNodeConfigurationController : BaseAPIController
    {
        RuntimeNodeConfigurationManager manager = new RuntimeNodeConfigurationManager();

        [HttpPost]
        [Route("GetFilteredRuntimeNodesConfigurations")]
        public object GetFilteredRuntimeNodesConfigurations(Vanrise.Entities.DataRetrievalInput<RuntimeNodeConfigurationQuery> input)
        { 
            return GetWebResponse(input, manager.GetFilteredRuntimeNodesConfigurations(input));
        }

        [HttpGet]
        [Route("GetNodeConfiguration")]
        public RuntimeNodeConfiguration GetNodeConfiguration(Guid nodeConfigurationId)
        {
            return manager.GetNodeConfiguration(nodeConfigurationId);
        }

        [HttpPost]
        [Route("AddRuntimeNodeConfiguration")]
        public Vanrise.Entities.InsertOperationOutput<RuntimeNodeConfigurationDetails> AddRuntimeNodeConfiguration(RuntimeNodeConfiguration nodeConfig)
        {
            return manager.AddRuntimeNodeConfiguration(nodeConfig);
        }

        [HttpPost]
        [Route("UpdateRuntimeNodeConfiguration")]
        public Vanrise.Entities.UpdateOperationOutput<RuntimeNodeConfigurationDetails> UpdateRuntimeNodeConfiguration(RuntimeNodeConfiguration nodeConfig)
        {
            return manager.UpdateRuntimeNodeConfiguration(nodeConfig);
        }

        [HttpGet]
        [Route("GetRuntimeServiceTypeTemplateConfigs")]
        public IEnumerable<RuntimeServiceConfig> GetRuntimeServiceTypeTemplateConfigs()
        {
            Vanrise.Runtime.Business.RuntimeNodeConfigurationManager runtimeServiceConfigsManager = new Vanrise.Runtime.Business.RuntimeNodeConfigurationManager();
            return runtimeServiceConfigsManager.GetRuntimeServiceTypeTemplateConfigs();
        }
    }
}