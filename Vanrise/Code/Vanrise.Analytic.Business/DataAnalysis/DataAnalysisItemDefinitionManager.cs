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
    public class DataAnalysisItemDefinitionManager
    {
        #region Public Methods

        public DataAnalysisItemDefinition GetDataAnalysisItemDefinition(Guid dataAnalysisItemDefinitionId)
        {
            Dictionary<Guid, DataAnalysisItemDefinition> cachedDataAnalysisItemDefinitions = this.GetCachedDataAnalysisItemDefinitions();
            return cachedDataAnalysisItemDefinitions.GetRecord(dataAnalysisItemDefinitionId);
        }

        public IDataRetrievalResult<DataAnalysisItemDefinitionDetail> GetFilteredDataAnalysisItemDefinitions(DataRetrievalInput<DataAnalysisItemDefinitionQuery> input)
        {
            var allDataAnalysisItemDefinitions = GetCachedDataAnalysisItemDefinitions();
            Func<DataAnalysisItemDefinition, bool> filterExpression = (x) =>
            {
                if (input.Query.DataAnalysisDefinitionId != x.DataAnalysisDefinitionId)
                    return false;

                if (input.Query.ItemDefinitionTypeId != x.Settings.ItemDefinitionTypeId)
                    return false;

                return true;
            };
                    

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allDataAnalysisItemDefinitions.ToBigResult(input, filterExpression, DataAnalysisItemDefinitionDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<DataAnalysisItemDefinitionDetail> AddDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataAnalysisItemDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();

            dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(dataAnalysisItemDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DataAnalysisItemDefinitionDetailMapper(dataAnalysisItemDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataAnalysisItemDefinitionDetail> UpdateDataAnalysisItemDefinition(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataAnalysisItemDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();

            if (dataManager.Update(dataAnalysisItemDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataAnalysisItemDefinitionDetailMapper(this.GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<DARecordAggregateConfig> GetDARecordAggregateExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DARecordAggregateConfig>(DARecordAggregateConfig.EXTENSION_TYPE);
        }

        public IEnumerable<TimeRangeFilterConfig> GetTimeRangeFilterExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<TimeRangeFilterConfig>(TimeRangeFilterConfig.EXTENSION_TYPE);
        }

        //public IEnumerable<DataAnalysisItemDefinitionInfo> GetDataAnalysisItemDefinitionsInfo(DataAnalysisItemDefinitionFilter filter)
        //{
        //    Func<DataAnalysisItemDefinition, bool> filterExpression = null;

        //    return this.GetCachedDataAnalysisItemDefinitions().MapRecords(DataAnalysisItemDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        //}

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataAnalysisItemDefinitionDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataAnalysisItemDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, DataAnalysisItemDefinition> GetCachedDataAnalysisItemDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataAnalysisItemDefinitions",
               () =>
               {
                   IDataAnalysisItemDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IDataAnalysisItemDefinitionDataManager>();
                   return dataManager.GetDataAnalysisItemDefinitions().ToDictionary(x => x.DataAnalysisItemDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        public DataAnalysisItemDefinitionDetail DataAnalysisItemDefinitionDetailMapper(DataAnalysisItemDefinition dataAnalysisItemDefinition)
        {
            DataAnalysisItemDefinitionDetail dataAnalysisItemDefinitionDetail = new DataAnalysisItemDefinitionDetail()
            {
                Entity = dataAnalysisItemDefinition
            };
            return dataAnalysisItemDefinitionDetail;
        }

        //public DataAnalysisItemDefinitionInfo DataAnalysisItemDefinitionInfoMapper(DataAnalysisItemDefinition dataAnalysisItemDefinition)
        //{
        //    DataAnalysisItemDefinitionInfo dataAnalysisItemDefinitionInfo = new DataAnalysisItemDefinitionInfo()
        //    {
        //        DataAnalysisItemDefinitionId = dataAnalysisItemDefinition.DataAnalysisItemDefinitionId,
        //        Name = dataAnalysisItemDefinition.Name
        //    };
        //    return dataAnalysisItemDefinitionInfo;
        //}

        #endregion   
    }
}
