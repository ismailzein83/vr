using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
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
		[Route("GetVRWorkflowVariablesTypeDescription")]
		public Dictionary<Guid, string> GetVRWorkflowVariablesTypeDescription(IEnumerable<VRWorkflowVariable> variables)
		{
			return _manager.GetVRWorkflowVariablesTypeDescription(variables);
		}

		[HttpPost]
		[Route("GetVRWorkflowArgumentTypeDescription")]
		public string GetVRWorkflowArgumentTypeDescription(VRWorkflowVariableType vrWorkflowArgumentType)
		{
			return _manager.GetVRWorkflowArgumentTypeDescription(vrWorkflowArgumentType);
		}
		
		[HttpPost]
		[Route("GetVRWorkflowVariableTypeDescription")]
		public string GetVRWorkflowVariableTypeDescription(VRWorkflowVariableType vrWorkflowVariableType)
		{
			return _manager.GetVRWorkflowVariableTypeDescription(vrWorkflowVariableType);
		}

		[HttpGet]
		[Route("GetVRWorkflowVariableTypeExtensionConfigs")]
		public IEnumerable<VRWorkflowVariableTypeConfig> GetVRWorkflowVariableTypeExtensionConfigs()
		{
			return _manager.GetVRWorkflowVariableTypeExtensionConfigs();
		}

		[HttpGet]
		[Route("GetVRWorkflowActivityExtensionConfigs")]
		public IEnumerable<VRWorkflowActivityConfig> GetVRWorkflowActivityExtensionConfigs()
		{
			return _manager.GetVRWorkflowActivityExtensionConfigs();
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

		[HttpPost]
		[Route("TryCompileWorkflow")]
		public VRWorkflowCompilationOutput TryCompileWorkflow(VRWorkflow vrWorkflow)
		{
			return _manager.TryCompileWorkflow(vrWorkflow);
		}

        //[HttpPost]
        //[Route("ExportCompilationResult")]
        //public object ExportCompilationResult(VRWorkflow vrWorkflow)
        //{
        //    VRWorkflowCompilationOutput result = TryCompileWorkflow(vrWorkflow);

        //    return base.GetExcelResponse(result.ErrorMessages.SelectMany(s => System.Text.Encoding.ASCII.GetBytes(s)).ToArray(), "CompilationResult.xls");
        //}

        [HttpGet]
        [Route("GetVRWorkflowsInfo")]
        public IEnumerable<VRWorkflowInfo> GetVRWorkflowsInfo(string filter = null)
        {
            VRWorkflowFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRWorkflowFilter>(filter) : null;
            return _manager.GetVRWorkflowsInfo(deserializedFilter);
        }
	}
}