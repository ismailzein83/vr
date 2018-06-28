using System;
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
        [HttpPost]
        [Route("GetFilteredVRWorkflows")]
        public object GetFilteredVRWorkflows(Vanrise.Entities.DataRetrievalInput<VRWorkflowQuery> input)
        {
            VRWorkflowManager manager = new VRWorkflowManager();
            return GetWebResponse(input, manager.GetFilteredVRWorkflows(input));
        }

        [HttpGet]
        [Route("GetVRWorkflow")]
        public VRWorkflow GetVRWorkflow(Guid vrWorkflowId)
        {
            VRWorkflowManager manager = new VRWorkflowManager();
            return manager.GetVRWorkflow(vrWorkflowId);
        }

        [HttpPost]
        [Route("InsertVRWorkflow")]
        public Vanrise.Entities.InsertOperationOutput<VRWorkflowDetail> InsertVRWorkflow(VRWorkflowToAdd vrWorkflow)
        {
            VRWorkflowManager manager = new VRWorkflowManager();
            return manager.InsertVRWorkflow(vrWorkflow);
        }

        [HttpPost]
        [Route("UpdateVRWorkflow")]
        public Vanrise.Entities.UpdateOperationOutput<VRWorkflowDetail> UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow)
        {
            VRWorkflowManager manager = new VRWorkflowManager();
            return manager.UpdateVRWorkflow(vrWorkflow);
        }

    }
}