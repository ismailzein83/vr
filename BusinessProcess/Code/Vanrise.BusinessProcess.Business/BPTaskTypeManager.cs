using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;

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
            return GetCachedBPTaskTypes().FindRecord(x => x.Name == taskTypeName);
        }

        //public BPTaskType GetBPTaskType(Guid taskTypeId)
        //{
        //    return GetCachedBPTaskTypesById().GetRecord(taskTypeId);
        //}
        //public BPTaskType GetBPTaskType(string taskTypeName)
        //{
        //    return GetCachedBPTaskTypesByName().GetRecord(taskTypeName);
        //}

        public IEnumerable<BPTaskTypeInfo> GetBPTaskTypesInfo(BPTaskTypeFilter filter)
        {
            var bpTaskTypes = GetCachedBPTaskTypes();
            Func<BPTaskType, bool> filterExpression = (itm) =>
            {
                if (filter != null)
                    return false;

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
        #endregion

        #region private methods
        private Dictionary<Guid, BPTaskType> GetCachedBPTaskTypesById()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBPTaskTypesById",
               () =>
               {
                   return GetBPTaskTypes().ToDictionary(cn => cn.BPTaskTypeId, cn => cn);
               });
        }

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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedBPTaskTypesByName",
               () =>
               {
                   return GetBPTaskTypes().ToDictionary(cn => cn.Name, cn => cn);
               });
        }

        private IEnumerable<BPTaskType> GetBPTaskTypes()
        {
            IBPTaskTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskTypeDataManager>();
            return dataManager.GetBPTaskTypes();
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPTaskTypeDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTaskTypeDataManager>();
            object _lastReceivedDataInfo;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPTaskTypesUpdated(ref _lastReceivedDataInfo);
            }
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