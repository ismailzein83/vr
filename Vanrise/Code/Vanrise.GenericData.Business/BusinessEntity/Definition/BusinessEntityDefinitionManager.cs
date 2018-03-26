using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityDefinitionManager : IBusinessEntityDefinitionManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<BusinessEntityDefinitionDetail> GetFilteredBusinessEntityDefinitions(Vanrise.Entities.DataRetrievalInput<BusinessEntityDefinitionQuery> input)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();

            Func<BusinessEntityDefinition, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(BusinessEntityDefinitionLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedBEDefinitions.ToBigResult(input, filterExpression, BusinessEntityDefinitionDetailMapper));
        }

        public Guid GetBusinessEntityDefinitionId(string businessEntityDefinitionName)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            var businessEntityDefinition = cachedBEDefinitions.FindRecord(x => x.Name == businessEntityDefinitionName);
            if (businessEntityDefinition == null)
                throw new NullReferenceException(String.Format("businessEntityDefinition. businessEntityDefinitionName '{0}'", businessEntityDefinitionName));
            return businessEntityDefinition.BusinessEntityDefinitionId;
        }
        public BusinessEntityDefinition GetBusinessEntityDefinition(Guid businessEntityDefinitionId)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            return cachedBEDefinitions.GetRecord(businessEntityDefinitionId);
        }

        public string GetBusinessEntityDefinitionName(Guid businessEntityDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            return beDefinition != null ? beDefinition.Title : null;
        }

        public IEnumerable<BusinessEntityDefinitionInfo> GetBusinessEntityDefinitionsInfo(BusinessEntityDefinitionInfoFilter filter)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            Func<BusinessEntityDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (item) =>
                {
                    if (filter.ExcludedIds != null && filter.ExcludedIds.Contains(item.BusinessEntityDefinitionId))
                        return false;

                    if (filter.Filters != null && !CheckIfFilterIsMatch(item, filter.Filters))
                        return false;

                    return true;
                }; 
            }

            return cachedBEDefinitions.MapRecords(BusinessEntityDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        public IEnumerable<BusinessEntityDefinitionInfo> GetRemoteBusinessEntityDefinitionsInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<IEnumerable<BusinessEntityDefinitionInfo>>(string.Format("/api/VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinitionsInfo?filter={0}", serializedFilter));
        }

        private struct GetBusinessEntityManagerCacheName
        {
            public Guid BusinessEntityDefinitionId { get; set; }
        }
        public BaseBusinessEntityManager GetBusinessEntityManager(Guid businessEntityDefinitionId)
        {
            var cacheName = new GetBusinessEntityManagerCacheName { BusinessEntityDefinitionId = businessEntityDefinitionId };// String.Format("GetBusinessEntityManager_{0}", businessEntityDefinitionId);
            Type beManagerType = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);

                    if (businessEntityDefinition == null)
                        throw new NullReferenceException("businessEntityDefinition");

                    if (businessEntityDefinition.Settings == null)
                        throw new NullReferenceException("businessEntityDefinition.Settings");

                    if (businessEntityDefinition.Settings.ManagerFQTN == null)
                        throw new NullReferenceException("businessEntityDefinition.Settings.ManagerFQTN");

                    Type beManagerType_Internal = Type.GetType(businessEntityDefinition.Settings.ManagerFQTN);

                    if (beManagerType_Internal == null)
                        throw new NullReferenceException("beManagerType_Internal");
                    return beManagerType_Internal;
                });

            var beManagerInstance = Activator.CreateInstance(beManagerType) as BaseBusinessEntityManager;
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("'{0}' does not implement IBusinessEntityManager", beManagerType.Name));

            return beManagerInstance;
        }

        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail> UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            UpdateOperationOutput<BusinessEntityDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateBusinessEntityDefinition(businessEntityDefinition);

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(BusinessEntityDefinitionLoggableEntity.Instance, businessEntityDefinition);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BusinessEntityDefinitionDetailMapper(businessEntityDefinition);
            }
            return updateOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail> AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            InsertOperationOutput<BusinessEntityDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            businessEntityDefinition.BusinessEntityDefinitionId = Guid.NewGuid();
            IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddBusinessEntityDefinition(businessEntityDefinition);
            if (insertActionSucc)
            {
                VRActionLogger.Current.TrackAndLogObjectAdded(BusinessEntityDefinitionLoggableEntity.Instance, businessEntityDefinition);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;

                insertOperationOutput.InsertedObject = BusinessEntityDefinitionDetailMapper(businessEntityDefinition);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            }

            return insertOperationOutput;
        }


        public IEnumerable<BusinessEntityDefinitionSettingsConfig> GetBEDefinitionSettingConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<BusinessEntityDefinitionSettingsConfig>(BusinessEntityDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public Guid? GetBEDataRecordTypeIdIfGeneric(Guid businessEntityDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null)
            {
                var genericBEDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
                if (genericBEDefinitionSettings != null)
                    return genericBEDefinitionSettings.DataRecordTypeId;
            }
            return null;
        }
        public string GetBusinessEntityNullDisplayText(Guid businessEntityDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            return beDefinition != null && beDefinition.Settings != null ? beDefinition.Settings.NullDisplayText : null;
        }

        public string GetBusinessEntityIdType(Guid remoteBEDefinitionId)
        {
            var businessEntityDefinition = GetBusinessEntityDefinition(remoteBEDefinitionId);
            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition of businessEntityDefinitionId: {0}", remoteBEDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings of businessEntityDefinitionId: {0}", remoteBEDefinitionId));

            return businessEntityDefinition.Settings.IdType;
        }
        public string GetBusinessEntityDefinitionViewEditor(Guid businessEntityDefinitionId)
        {
            var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", businessEntityDefinitionId);
            businessEntityDefinition.Settings.ThrowIfNull("businessEntityDefinition.Settings",businessEntityDefinitionId);
            return businessEntityDefinition.Settings.ViewerEditor;
        }
        public Dictionary<Guid, BusinessEntityDefinition> GetBusinessEntityDefinitionsByConfigId(Guid beConfigId)
        {
            return GetCachedBusinessEntityDefinitionsByConfigId().GetRecord(beConfigId);
        }

        #endregion

        #region Private Methods

        public Dictionary<Guid, BusinessEntityDefinition> GetCachedBusinessEntityDefinitions()
        {
            return s_cacheManager.GetOrCreateObject("GetCachedBusinessEntityDefinitions",
                () =>
                {
                    IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
                    IEnumerable<BusinessEntityDefinition> beDefinitions = dataManager.GetBusinessEntityDefinitions();
                    return beDefinitions.ToDictionary(beDefinition => beDefinition.BusinessEntityDefinitionId, beDefinition => beDefinition);
                });
        }

        private Dictionary<Guid, Dictionary<Guid, BusinessEntityDefinition>> GetCachedBusinessEntityDefinitionsByConfigId()
        {
            return s_cacheManager.GetOrCreateObject("GetCachedBusinessEntityDefinitionsByConfigId",
                () =>
                {
                    Dictionary<Guid, Dictionary<Guid, BusinessEntityDefinition>> rslt = new Dictionary<Guid, Dictionary<Guid, BusinessEntityDefinition>>();
                    var allBEDefinitions = GetCachedBusinessEntityDefinitions();
                    if(allBEDefinitions != null)
                    {
                        foreach(var beDefinition in allBEDefinitions.Values)
                        {
                            beDefinition.Settings.ThrowIfNull("beDefinition.Settings", beDefinition.BusinessEntityDefinitionId);
                            rslt.GetOrCreateItem(beDefinition.Settings.ConfigId).Add(beDefinition.BusinessEntityDefinitionId, beDefinition);
                        }
                    }
                    return rslt;
                });
        }

        private bool CheckIfFilterIsMatch(BusinessEntityDefinition entityDefinition, List<IBusinessEntityDefinitionFilter> filters)
        {
            BusinessEntityDefinitionFilterContext context = new BusinessEntityDefinitionFilterContext { entityDefinition = entityDefinition };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        private class BusinessEntityDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static BusinessEntityDefinitionLoggableEntity Instance = new BusinessEntityDefinitionLoggableEntity();

            private BusinessEntityDefinitionLoggableEntity()
            {

            }

            static BusinessEntityDefinitionManager s_businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_BusinessEntityDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Business Entity Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_BusinessEntityDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                BusinessEntityDefinition businessEntityDefinition = context.Object.CastWithValidate<BusinessEntityDefinition>("context.Object");
                return businessEntityDefinition.BusinessEntityDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                BusinessEntityDefinition businessEntityDefinition = context.Object.CastWithValidate<BusinessEntityDefinition>("context.Object");
                return s_businessEntityDefinitionManager.GetBusinessEntityDefinitionName(businessEntityDefinition.BusinessEntityDefinitionId);
            }
        }

        #endregion

        #region Caching

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        public T GetCachedOrCreate<T>(Object cacheName, Func<T> createObject)
        {
            return s_cacheManager.GetOrCreateObject(cacheName, createObject);
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBusinessEntityDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        BusinessEntityDefinitionInfo BusinessEntityDefinitionInfoMapper(BusinessEntityDefinition beDefinition)
        {
            return new BusinessEntityDefinitionInfo()
            {
                BusinessEntityDefinitionId = beDefinition.BusinessEntityDefinitionId,
                Name = beDefinition.Title,
                SelectorFilterEditor = beDefinition.Settings.SelectorFilterEditor
            };
        }

        BusinessEntityDefinitionDetail BusinessEntityDefinitionDetailMapper(BusinessEntityDefinition beDefinition)
        {
            Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);
            bool isExtensible = typeof(ExtensibleBEManager).IsAssignableFrom(beManagerType);
            return new BusinessEntityDefinitionDetail()
            {
                Entity = beDefinition,
                IsExtensible = isExtensible
            };
        }

        #endregion
    }
}
