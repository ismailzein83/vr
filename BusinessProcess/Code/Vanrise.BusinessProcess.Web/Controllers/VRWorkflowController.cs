using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRWorkflow")]
    public class VRWorkflowController : BaseAPIController
    {
        VRWorkflowManager _manager = new VRWorkflowManager();

        [HttpPost]
        [Route("GetFilteredVRWorkflows")]
        public object GetFilteredVRWorkflows(Vanrise.Entities.DataRetrievalInput<VRWorkflowQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRWorkflows(input));
        }

        [HttpGet]
        [Route("GetVRWorkflowEditorRuntime")]
        public VRWorkflowEditorRuntime GetVRWorkflowEditorRuntime(Guid vrWorkflowId)
        {
            return _manager.GetVRWorkflowEditorRuntime(vrWorkflowId);
        }

        [HttpPost]
        [Route("GetVRWorkflowArgumentTypeDescription")]
        public string GetVRWorkflowArgumentTypeDescription(VRWorkflowVariableType vrWorkflowArgumentType)
        {
            return _manager.GetVRWorkflowArgumentTypeDescription(vrWorkflowArgumentType);
        }

        [HttpGet]
        [Route("GetVRWorkflowVariableTypeExtensionConfigs")]
        public IEnumerable<VRWorkflowVariableTypeConfig> GetVRWorkflowVariableTypeExtensionConfigs()
        {
            return _manager.GetVRWorkflowVariableTypeExtensionConfigs();
        }

        [HttpPost]
        [Route("InsertVRWorkflow")]
        public Vanrise.Entities.InsertOperationOutput<VRWorkflowDetail> InsertVRWorkflow(VRWorkflowToAdd vrWorkflow)
        {
            return _manager.InsertVRWorkflow(vrWorkflow);
        }

        [HttpPost]
        [Route("UpdateVRWorkflow")]
        public Vanrise.Entities.UpdateOperationOutput<VRWorkflowDetail> UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow)
        {
            return _manager.UpdateVRWorkflow(vrWorkflow);
        }
    }
}