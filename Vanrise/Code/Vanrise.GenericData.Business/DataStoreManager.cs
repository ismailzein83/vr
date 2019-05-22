﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataStoreManager
    {
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #region Public Methods
        public IEnumerable<DataStoreConfig> GetDataStoreConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DataStoreConfig>(DataStoreConfig.EXTENSION_TYPE);
        }
        public IEnumerable<DataStoreInfo> GetDataStoresInfo()
        {
            Func<DataStore, bool> filterExpression = (dataStore) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(dataStore.DevProjectId))
                    return false;

                return true;
            };
            return this.GetCachedDataStores().MapRecords(DataStoreInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataStoreDetail> GetFilteredDataStores(Vanrise.Entities.DataRetrievalInput<DataStoreQuery> input)
        {
            var cachedDataStore = GetCachedDataStores();
            Func<DataStore, bool> filterExpression = (dataStore) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(dataStore.DevProjectId))
                    return false;

                if (input.Query.Name != null && !dataStore.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                    return false;
                if (input.Query.DevProjectIds != null && (!dataStore.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(dataStore.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(DataStoreLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataStore.ToBigResult(input, filterExpression, DataStoreDetailMapper));
        }

        public DataStore GetDataStore(Guid dataStoreId)
        {
            var cachedDataStore = GetCachedDataStores();

            return cachedDataStore.FindRecord((dataStore) => dataStore.DataStoreId == dataStoreId);
        }

       
        public string GetDataStoreName(DataStore dataStore)
        {
            if (dataStore != null)
                return dataStore.Name;
            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<DataStoreDetail> AddDataStore(DataStore dataStore)
        {
            InsertOperationOutput<DataStoreDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataStoreDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            dataStore.DataStoreId = Guid.NewGuid();

            IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            bool insertActionSucc = dataManager.AddDataStore(dataStore);

            if (insertActionSucc)
            {
                VRActionLogger.Current.TrackAndLogObjectAdded(DataStoreLoggableEntity.Instance, dataStore);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DataStoreDetailMapper(dataStore);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataStoreDetail> UpdateDataStore(DataStore dataStore)
        {
            UpdateOperationOutput<DataStoreDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataStoreDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            bool updateActionSucc = dataManager.UpdateDataStore(dataStore);

            if (updateActionSucc)
            {
                VRActionLogger.Current.TrackAndLogObjectUpdated(DataStoreLoggableEntity.Instance, dataStore);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = DataStoreDetailMapper(dataStore);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, DataStore> GetCachedDataStores()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataStores",
                () =>
                {
                    IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
                    IEnumerable<DataStore> dataStores = dataManager.GetDataStores();
                    return dataStores.ToDictionary(dataStore => dataStore.DataStoreId, dataStore => dataStore);
                });
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataStoreDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataStoresUpdated(ref _updateHandle);
            }
        }
        private class DataStoreLoggableEntity : VRLoggableEntityBase
        {
            public static DataStoreLoggableEntity Instance = new DataStoreLoggableEntity();

            private DataStoreLoggableEntity()
            {

            }

            static DataStoreManager s_dataStoreManager = new DataStoreManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_DataStore"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Data Store"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_DataStore_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataStore dataStore = context.Object.CastWithValidate<DataStore>("context.Object");
                return dataStore.DataStoreId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataStore dataStore = context.Object.CastWithValidate<DataStore>("context.Object");
                return s_dataStoreManager.GetDataStoreName(dataStore);
            }
        }
        #endregion

        #region Mappers

        DataStoreInfo DataStoreInfoMapper(DataStore dataStore)
        {
            return new DataStoreInfo() {
                 DataStoreId = dataStore.DataStoreId,
                 Name = dataStore.Name
            };
        }

        DataStoreDetail DataStoreDetailMapper(DataStore dataStore)
        {
            string devProjectName = null;
            if (dataStore.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(dataStore.DevProjectId.Value);
            }
            return new DataStoreDetail()
            {
                Entity = dataStore,
                DevProjectName=devProjectName
            };
        }

        #endregion
    }
}
