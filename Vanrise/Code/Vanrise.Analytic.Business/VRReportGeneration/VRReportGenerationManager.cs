//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Vanrise.Analytic.Data;
//using Vanrise.Analytic.Entities;
//using Vanrise.Entities;
//using Vanrise.Common;
//using Vanrise.Common.Business;
//using Vanrise.GenericData.Business;

//namespace Vanrise.Analytic.Business
//{
//    public class VRReportManager
//    {
//        #region Public Methods

//        public DataAnalysisItemDefinition GetDataAnalysisItemDefinition(Guid dataAnalysisItemDefinitionId)
//        {
//            Dictionary<Guid, DataAnalysisItemDefinition> cachedDataAnalysisItemDefinitions = this.GetCachedDataAnalysisItemDefinitions();
//            return cachedDataAnalysisItemDefinitions.GetRecord(dataAnalysisItemDefinitionId);
//        }

//        public IEnumerable<DataAnalysisItemDefinition> GetDataAnalysisItemDefinitionsById(Guid dataAnalysisDefinitionId)
//        {
//            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataAnalysisItemDefinitionsById",
//               () =>
//               {
//                   var allDataAnalysisItemDefinitions = GetCachedDataAnalysisItemDefinitions();
//                   Func<DataAnalysisItemDefinition, bool> filterExpression = (x) =>
//                   {
//                       if (dataAnalysisDefinitionId != x.DataAnalysisDefinitionId)
//                           return false;

//                       return true;
//                   };
//                   return allDataAnalysisItemDefinitions != null ? allDataAnalysisItemDefinitions.FindAllRecords(filterExpression) : null;
//               });
//        }

//        public T GetDataAnalysisItemDefinitionSettings<T>(Guid dataAnalysisItemDefinitionId) where T : DataAnalysisItemDefinitionSettings
//        {
//            DataAnalysisItemDefinition dataAnalysisItemDefinition = GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId);
//            if (dataAnalysisItemDefinition == null)
//                throw new NullReferenceException(string.Format("dataAnalysisItemDefinition. OutputItemDefinitionId: {0}", dataAnalysisItemDefinitionId));

//            if (dataAnalysisItemDefinition.Settings == null)
//                throw new NullReferenceException(string.Format("dataAnalysisItemDefinition.Settings. OutputItemDefinitionId: {0}", dataAnalysisItemDefinitionId));

//            T settings = dataAnalysisItemDefinition.Settings as T;
//            if (settings == null)
//                throw new Exception(String.Format("dataAnalysisItemDefinition.Settings is not of type {0}. it is of type {1}", typeof(T), dataAnalysisItemDefinition.Settings.GetType()));

//            return settings;
//        }

//        public IDataRetrievalResult<DataAnalysisItemDefinitionDetail> GetFilteredDataAnalysisItemDefinitions(DataRetrievalInput<DataAnalysisItemDefinitionQuery> input)
//        {
//            var allDataAnalysisItemDefinitions = GetCachedDataAnalysisItemDefinitions();
//            Func<DataAnalysisItemDefinition, bool> filterExpression = (x) =>
//            {
//                if (input.Query.DataAnalysisDefinitionId != x.DataAnalysisDefinitionId)
//                    return false;

//                if (input.Query.ItemDefinitionTypeId != x.Settings.ItemDefinitionTypeId)
//                    return false;

//                return true;
//            };

//            VRActionLogger.Current.LogGetFilteredAction(DataAnalysisItemDefinitionLoggableEntity.Instance, input);
//            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDataAnalysisItemDefinitions.ToBigResult(input, filterExpression, DataAnalysisItemDefinitionDetailMapper));
//        }

//        public string GetDataAnalysisItemDefinitionName(DataAnalysisItemDefinition dataAnalysisItemDefinition)
//        {
//            if (dataAnalysisItemDefinition != null)
//                return dataAnalysisItemDefinition.Name;
//            return null;
//        }
//        public Vanrise.Entities.InsertOperationOutput<DataAnalysisItemDefinitionDetail> AddDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
//        {
//            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataAnalysisItemDefinitionDetail>();

//            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
//            insertOperationOutput.InsertedObject = null;

//            IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();

//            dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId = Guid.NewGuid();

//            if (dataManager.Insert(dataAnalysisItemDefinitionItem))
//            {
//                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
//                DataRecordTypeManager.CacheManager.TriggerCacheExpiration();
//                VRActionLogger.Current.TrackAndLogObjectAdded(DataAnalysisItemDefinitionLoggableEntity.Instance, dataAnalysisItemDefinitionItem);
//                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
//                insertOperationOutput.InsertedObject = DataAnalysisItemDefinitionDetailMapper(dataAnalysisItemDefinitionItem);
//            }
//            else
//            {
//                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
//            }

//            return insertOperationOutput;
//        }

//        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisItemDefinitionDetail> UpdateDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
//        {
//            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataAnalysisItemDefinitionDetail>();

//            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
//            updateOperationOutput.UpdatedObject = null;

//            IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();

//            if (dataManager.Update(dataAnalysisItemDefinitionItem))
//            {
//                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
//                DataRecordTypeManager.CacheManager.TriggerCacheExpiration();
//                VRActionLogger.Current.TrackAndLogObjectUpdated(DataAnalysisItemDefinitionLoggableEntity.Instance, dataAnalysisItemDefinitionItem);
//                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
//                updateOperationOutput.UpdatedObject = DataAnalysisItemDefinitionDetailMapper(this.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId));
//            }
//            else
//            {
//                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
//            }

//            return updateOperationOutput;
//        }

//        public IEnumerable<DARecordAggregateConfig> GetDARecordAggregateExtensionConfigs()
//        {
//            var templateConfigManager = new ExtensionConfigurationManager();
//            return templateConfigManager.GetExtensionConfigurations<DARecordAggregateConfig>(DARecordAggregateConfig.EXTENSION_TYPE);
//        }

//        public IEnumerable<TimeRangeFilterConfig> GetTimeRangeFilterExtensionConfigs()
//        {
//            var templateConfigManager = new ExtensionConfigurationManager();
//            return templateConfigManager.GetExtensionConfigurations<TimeRangeFilterConfig>(TimeRangeFilterConfig.EXTENSION_TYPE);
//        }

//        public IEnumerable<DataAnalysisItemDefinitionInfo> GetDataAnalysisItemDefinitionsInfo(DataAnalysisItemDefinitionFilter filter, Guid dataAnalysisDefinitonId)
//        {
//            Func<DataAnalysisItemDefinition, bool> filterExpression = (dataAnalysisItemDefinition) =>
//            {
//                if (dataAnalysisItemDefinition.DataAnalysisDefinitionId != dataAnalysisDefinitonId)
//                    return false;

//                if (filter != null)
//                {
//                    if (filter.Filters != null)
//                    {
//                        foreach (IDataAnalysisItemDefinitionFilter dataAnalysisItemDefinitionFilter in filter.Filters)
//                        {
//                            if (!dataAnalysisItemDefinitionFilter.IsMatch(dataAnalysisItemDefinition))
//                                return false;
//                        }
//                    }
//                }
//                return true;
//            };

//            return this.GetCachedDataAnalysisItemDefinitions().MapRecords(DataAnalysisItemDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
//        }

//        #endregion

//        #region Private Classes

//        public class CacheManager : Vanrise.Caching.BaseCacheManager
//        {
//            IDataAnalysisItemDefinitionDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();
//            object _updateHandle;

//            protected override bool ShouldSetCacheExpired(object parameter)
//            {
//                return _dataManager.AreDataAnalysisItemDefinitionUpdated(ref _updateHandle);
//            }
//        }

//        private class DataAnalysisItemDefinitionLoggableEntity : VRLoggableEntityBase
//        {
//            public static DataAnalysisItemDefinitionLoggableEntity Instance = new DataAnalysisItemDefinitionLoggableEntity();

//            private DataAnalysisItemDefinitionLoggableEntity()
//            {

//            }

//            static DataAnalysisItemDefinitionManager s_dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();

//            public override string EntityUniqueName
//            {
//                get { return "VR_Analytic_DataAnalysisItemDefinition"; }
//            }

//            public override string ModuleName
//            {
//                get { return "Analytic"; }
//            }

//            public override string EntityDisplayName
//            {
//                get { return "Data Analysis Item Definition"; }
//            }

//            public override string ViewHistoryItemClientActionName
//            {
//                get { return "VR_Analytic_DataAnalysisItemDefinition_ViewHistoryItem"; }
//            }

//            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
//            {
//                DataAnalysisItemDefinition dataAnalysisItemDefinition = context.Object.CastWithValidate<DataAnalysisItemDefinition>("context.Object");
//                return dataAnalysisItemDefinition.DataAnalysisItemDefinitionId;
//            }

//            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
//            {
//                DataAnalysisItemDefinition dataAnalysisItemDefinition = context.Object.CastWithValidate<DataAnalysisItemDefinition>("context.Object");
//                return s_dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionName(dataAnalysisItemDefinition);
//            }
//        }

//        #endregion

//        #region Private Methods

//        Dictionary<Guid, DataAnalysisItemDefinition> GetCachedDataAnalysisItemDefinitions()
//        {
//            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataAnalysisItemDefinitions",
//               () =>
//               {
//                   IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();
//                   return dataManager.GetDataAnalysisItemDefinitions().ToDictionary(x => x.DataAnalysisItemDefinitionId, x => x);
//               });
//        }

//        #endregion

//        #region Mappers

//        public DataAnalysisItemDefinitionDetail DataAnalysisItemDefinitionDetailMapper(DataAnalysisItemDefinition dataAnalysisItemDefinition)
//        {
//            DataAnalysisItemDefinitionDetail dataAnalysisItemDefinitionDetail = new DataAnalysisItemDefinitionDetail()
//            {
//                Entity = dataAnalysisItemDefinition
//            };
//            return dataAnalysisItemDefinitionDetail;
//        }

//        public DataAnalysisItemDefinitionInfo DataAnalysisItemDefinitionInfoMapper(DataAnalysisItemDefinition dataAnalysisItemDefinition)
//        {
//            DataAnalysisItemDefinitionInfo dataAnalysisItemDefinitionInfo = new DataAnalysisItemDefinitionInfo()
//            {
//                DataAnalysisItemDefinitionId = dataAnalysisItemDefinition.DataAnalysisItemDefinitionId,
//                Name = dataAnalysisItemDefinition.Name
//            };
//            return dataAnalysisItemDefinitionInfo;
//        }

//        #endregion
//    }
//}