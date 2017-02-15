﻿using System;
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
                filterExpression = (item) => (filter.Filters != null && CheckIfFilterIsMatch(item, filter.Filters));
            }

            return cachedBEDefinitions.MapRecords(BusinessEntityDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        //public IEnumerable<BusinessEntityDefinitionInfo> GetRemoteBusinessEntityDefinitionsInfo(Guid connectionId, BusinessEntityDefinitionInfoFilter filter)
        //{
        //    VRConnectionManager connectionManager = new VRConnectionManager();
        //    var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
        //    VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

        //    return connectionSettings.Get<IEnumerable<BusinessEntityDefinitionInfo>>(string.Format("/api/VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinitionsInfo?filter={0}", filter));
        //}

        private struct GetBusinessEntityManagerCacheName
        {
            public Guid BusinessEntityDefinitionId { get; set; }
        }
        public IBusinessEntityManager GetBusinessEntityManager(Guid businessEntityDefinitionId)
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

            var beManagerInstance = Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
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
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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

        public Vanrise.Security.Entities.View GetGenericBEDefinitionView(Guid businessEntityDefinitionId)
        {
            var viewManager = new Vanrise.Security.Business.ViewManager();
            var allViews = viewManager.GetViews();
            return allViews.FirstOrDefault(v => (v.Settings as GenericBEViewSettings) != null && (v.Settings as GenericBEViewSettings).BusinessEntityDefinitionId == businessEntityDefinitionId);
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

        #endregion

        #region Private Methods

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

        #endregion

        #region Caching

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        public T GetCachedOrCreate<T>(Object cacheName, Func<T> createObject)
        {
            return s_cacheManager.GetOrCreateObject(cacheName, createObject);
        }

        private Dictionary<Guid, BusinessEntityDefinition> GetCachedBusinessEntityDefinitions()
        {
            return s_cacheManager.GetOrCreateObject("GetBusinessEntityDefinitions",
                () =>
                {
                    IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
                    IEnumerable<BusinessEntityDefinition> beDefinitions = dataManager.GetBusinessEntityDefinitions();
                    return beDefinitions.ToDictionary(beDefinition => beDefinition.BusinessEntityDefinitionId, beDefinition => beDefinition);
                });
        }

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
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
