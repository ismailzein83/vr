using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPTaskType")]
    public class BPTaskTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBPTaskTypeByTaskId")]
        public BPTaskType GetBPTaskTypeByTaskId(long taskId)
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBPTaskTypeByTaskId(taskId);
        }
        [HttpGet]
        [Route("GetBaseBPTaskTypeSettingsConfigs")]
        public object GetBaseBPTaskTypeSettingsConfigs()
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBaseBPTaskTypeSettingsConfigs();
        }

        [HttpGet]
        [Route("GetBPTaskType")]
        public BPTaskType GetBPTaskType(Guid taskTypeId)
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBPTaskType(taskTypeId);
        }


        [HttpGet]
        [Route("GetBPTaskTypesInfo")]
        public IEnumerable<BPTaskTypeInfo> GetBPTaskTypesInfo(string filter = null)
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            BPTaskTypeFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<BPTaskTypeFilter>(filter) : null;
            return manager.GetBPTaskTypesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetVRWorkflowTaskAssigneesSettingExtensionConfigs")]
        public IEnumerable<VRWorkflowTaskAssigneesSettingConfig> GetVRWorkflowTaskAssigneesSettingExtensionConfigs()
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetVRWorkflowTaskAssigneesSettingExtensionConfigs();
        }

        [HttpGet]
        [Route("GetBPGenericTaskTypeActionSettingsExtensionConfigs")]
        public IEnumerable<BPGenericTaskTypeActionSettingsConfig> GetBPGenericTaskTypeActionSettingsExtensionConfigs()
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBPGenericTaskTypeActionSettingsExtensionConfigs();
        }

        [HttpGet]
        [Route("GetBPGenericTaskTypeActionFilterConditionExtensionConfigs")]
        public IEnumerable<BPGenericTaskTypeActionFilterConditionConfig> GetBPGenericTaskTypeActionFilterConditionExtensionConfigs()
        {
            BPTaskTypeManager manager = new BPTaskTypeManager();
            return manager.GetBPGenericTaskTypeActionFilterConditionExtensionConfigs();
        }
    }
}