using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.Business
{
    public class DataAnalysisDefinitionManager
    {
        #region Public Methods

        public DataAnalysisDefinition GetDataAnalysisDefinition(Guid dataAnalysisDefinitionId)
        {
            Dictionary<Guid, DataAnalysisDefinition> cachedDataAnalysisDefinitions = this.GetCachedDataAnalysisDefinitions();
            return cachedDataAnalysisDefinitions.GetRecord(dataAnalysisDefinitionId);
        }

        public IDataRetrievalResult<DataAnalysisDefinitionDetail> GetFilteredDataAnalysisDefinitions(DataRetrievalInput<DataAnalysisDefinitionQuery> input)
        {
            var allDataAnalysisDefinitions = GetCachedDataAnalysisDefinitions();
            Func<DataAnalysisDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(DataAnalysisDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDataAnalysisDefinitions.ToBigResult(input, filterExpression, DataAnalysisDefinitionDetailMapper));
        }

        public string GetDataAnalysisDefinitionName(DataAnalysisDefinition dataAnalysisDefinition)
        {
            if (dataAnalysisDefinition != null)
              return  dataAnalysisDefinition.Name;
            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<DataAnalysisDefinitionDetail> AddDataAnalysisDefinition(DataAnalysisDefinition dataAnalysisDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataAnalysisDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IDataAnalysisDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisDefinitionDataManager>();

            dataAnalysisDefinitionItem.DataAnalysisDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(dataAnalysisDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(DataAnalysisDefinitionLoggableEntity.Instance, dataAnalysisDefinitionItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DataAnalysisDefinitionDetailMapper(dataAnalysisDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisDefinitionDetail> UpdateDataAnalysisDefinition(DataAnalysisDefinition dataAnalysisDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataAnalysisDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDataAnalysisDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisDefinitionDataManager>();

            if (dataManager.Update(dataAnalysisDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(DataAnalysisDefinitionLoggableEntity.Instance, dataAnalysisDefinitionItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataAnalysisDefinitionDetailMapper(this.GetDataAnalysisDefinition(dataAnalysisDefinitionItem.DataAnalysisDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<DataAnalysisDefinitionConfig> GetDataAnalysisDefinitionSettingsExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DataAnalysisDefinitionConfig>(DataAnalysisDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<DataAnalysisDefinitionInfo> GetDataAnalysisDefinitionsInfo(DataAnalysisDefinitionFilter filter)
        {
            Func<DataAnalysisDefinition, bool> filterExpression = (dataAnalysisDefinition) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null)
                    {
                        foreach (IDataAnalysisDefinitionFilter dataAnalysisDefinitionFilter in filter.Filters)
                        {
                            if (!dataAnalysisDefinitionFilter.IsMatch(dataAnalysisDefinition))
                                return false;
                        }
                    }
                }
                return true;
            };

            return this.GetCachedDataAnalysisDefinitions().MapRecords(DataAnalysisDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public T GetDataAnalysisDefinitionSettings<T>(Guid dataAnalysisDefinitionId) where T : DataAnalysisDefinitionSettings
        {
            DataAnalysisDefinition dataAnalysisDefinition = GetDataAnalysisDefinition(dataAnalysisDefinitionId);
            if (dataAnalysisDefinition == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition. DataAnalysisDefinitionId: {0}", dataAnalysisDefinitionId));

            if (dataAnalysisDefinition.Settings == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition.Settings. DataAnalysisDefinitionId: {0}", dataAnalysisDefinitionId));

            T settings = dataAnalysisDefinition.Settings as T;
            if (settings == null)
                throw new Exception(String.Format("dataAnalysisDefinition.Settings is not of type {0}. it is of type {1}", typeof(T), dataAnalysisDefinition.Settings.GetType()));

            return settings;
        }
        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataAnalysisDefinitionDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataAnalysisDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, DataAnalysisDefinition> GetCachedDataAnalysisDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataAnalysisDefinitions",
               () =>
               {
                   IDataAnalysisDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisDefinitionDataManager>();
                   return dataManager.GetDataAnalysisDefinitions().ToDictionary(x => x.DataAnalysisDefinitionId, x => x);
               });
        }

        #endregion
        private class DataAnalysisDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static DataAnalysisDefinitionLoggableEntity Instance = new DataAnalysisDefinitionLoggableEntity();

            private DataAnalysisDefinitionLoggableEntity()
            {

            }

            static DataAnalysisDefinitionManager s_dataAnalysisDefinitionManager = new DataAnalysisDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_Analytic_DataAnalysisDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Analytic"; }
            }

            public override string EntityDisplayName
            {
                get { return "Data Analysis Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Analytic_DataAnalysisDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataAnalysisDefinition dataAnalysisDefinition = context.Object.CastWithValidate<DataAnalysisDefinition>("context.Object");
                return dataAnalysisDefinition.DataAnalysisDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataAnalysisDefinition dataAnalysisDefinition = context.Object.CastWithValidate<DataAnalysisDefinition>("context.Object");
                return s_dataAnalysisDefinitionManager.GetDataAnalysisDefinitionName(dataAnalysisDefinition);
            }
        }

        #region Mappers

        public DataAnalysisDefinitionDetail DataAnalysisDefinitionDetailMapper(DataAnalysisDefinition dataAnalysisDefinition)
        {
            DataAnalysisDefinitionDetail dataAnalysisDefinitionDetail = new DataAnalysisDefinitionDetail()
            {
                Entity = dataAnalysisDefinition
            };
            return dataAnalysisDefinitionDetail;
        }

        public DataAnalysisDefinitionInfo DataAnalysisDefinitionInfoMapper(DataAnalysisDefinition dataAnalysisDefinition)
        {
            DataAnalysisDefinitionInfo dataAnalysisDefinitionInfo = new DataAnalysisDefinitionInfo()
            {
                DataAnalysisDefinitionId = dataAnalysisDefinition.DataAnalysisDefinitionId,
                Name = dataAnalysisDefinition.Name
            };
            return dataAnalysisDefinitionInfo;
        }

        #endregion
    }
}
