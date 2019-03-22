using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTaskTypeManager
    {
        static Guid businessEntityDefinitionId = new Guid("d33fd65a-721f-4ae1-9d41-628be9425796");

        #region public methods

        public BPTaskType GetBPTaskType(Guid taskTypeId)
        {
            return GetCachedBPTaskTypes().GetRecord(taskTypeId);
        }

        public BPTaskType GetBPTaskType(string taskTypeName)
        {
            return GetCachedBPTaskTypesByName().GetRecord(taskTypeName);
        }

        public IEnumerable<BPTaskTypeInfo> GetBPTaskTypesInfo(BPTaskTypeFilter filter)
        {
            var bpTaskTypes = GetCachedBPTaskTypes();
            Func<BPTaskType, bool> filterExpression = (itm) =>
            {
                if (filter != null && filter.Filters != null)
                {
                    foreach (IBPTaskTypeSettingsFilter bptaskTypeSettingsFilter in filter.Filters)
                    {
                        var context = new BPTaskTypeSettingsFilterContext() { BPTaskType = itm};
                        if (!bptaskTypeSettingsFilter.IsMatch(context))
                            return false;
                    }
                }

                return true;
            };
            return bpTaskTypes.MapRecords(BPTaskTypeInfoMapper, filterExpression);
        }

        public BPTaskType GetBPTaskTypeByTaskId(long taskId)
        {
            BPTaskManager bpTaskManager = new BPTaskManager();
            var bpTask = bpTaskManager.GetTask(taskId);
            return GetBPTaskType(bpTask.TypeId);
        }

        public IEnumerable<BaseBPTaskTypeSettingsConfig> GetBaseBPTaskTypeSettingsConfigs()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<BaseBPTaskTypeSettingsConfig>(BaseBPTaskTypeSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRWorkflowTaskAssigneesSettingConfig> GetVRWorkflowTaskAssigneesSettingExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRWorkflowTaskAssigneesSettingConfig>(VRWorkflowTaskAssigneesSettingConfig.EXTENSION_TYPE);
        }

        public IEnumerable<BPGenericTaskTypeActionSettingsConfig> GetBPGenericTaskTypeActionSettingsExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<BPGenericTaskTypeActionSettingsConfig>(BPGenericTaskTypeActionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<BPGenericTaskTypeActionFilterConditionConfig> GetBPGenericTaskTypeActionFilterConditionExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<BPGenericTaskTypeActionFilterConditionConfig>(BPGenericTaskTypeActionFilterConditionConfig.EXTENSION_TYPE);
        }
        #endregion

        #region private methods
        private Dictionary<Guid, BPTaskType> GetCachedBPTaskTypes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedBPTaskTypes", businessEntityDefinitionId, () =>
            {
                Dictionary<Guid, BPTaskType> result = new Dictionary<Guid, BPTaskType>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        BPTaskType bpTaskType = new BPTaskType()
                        {
                            BPTaskTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("BPTaskTypeId"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Settings = genericBusinessEntity.FieldValues.GetRecord("Settings") as BaseBPTaskTypeSettings,
                        };
                        result.Add(bpTaskType.BPTaskTypeId, bpTaskType);
                    }
                }
                return result;
            });
        }

        private Dictionary<string, BPTaskType> GetCachedBPTaskTypesByName()
        {
            return GetCachedBPTaskTypes().Values.ToDictionary(cn => cn.Name, cn => cn);
        }
        #endregion

        #region Mappers
        private BPTaskTypeInfo BPTaskTypeInfoMapper(BPTaskType bPTaskType)
        {
            return new BPTaskTypeInfo()
            {
                BPTaskTypeId = bPTaskType.BPTaskTypeId,
                Name = bPTaskType.Name,
            };
        }
        #endregion
    }
}