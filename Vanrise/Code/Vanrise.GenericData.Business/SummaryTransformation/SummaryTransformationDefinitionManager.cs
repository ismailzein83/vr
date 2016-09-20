using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class SummaryTransformationDefinitionManager
    {

        #region Public Methods
        public IDataRetrievalResult<SummaryTransformationDefinitionDetail> GetFilteredSummaryTransformationDefinitions(DataRetrievalInput<SummaryTransformationDefinitionQuery> input)
        {
            var allItems = GetCachedSummaryTransformationDefinitions();

            Func<SummaryTransformationDefinition, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, SummaryTransformationDefinitionDetailMapper));
        }
        public SummaryTransformationDefinition GetSummaryTransformationDefinition(int summaryTransformationDefinitionId)
        {
            var summaryTransformationDefinitions = GetCachedSummaryTransformationDefinitions();
            return summaryTransformationDefinitions.GetRecord(summaryTransformationDefinitionId);
        }
        public IEnumerable<SummaryTransformationDefinitionInfo> GetSummaryTransformationDefinitionInfo(SummaryTransformationDefinitionInfoFilter filter)
        {
            var summaryTransformationDefinitions = GetCachedSummaryTransformationDefinitions();
            if (filter != null)
            {
                Func<SummaryTransformationDefinition, bool> filterExpression = (x) =>
                     (!filter.RawItemRecordTypeId.HasValue || (filter.RawItemRecordTypeId.HasValue && filter.RawItemRecordTypeId.Value == x.RawItemRecordTypeId))
                    && (!filter.SummaryItemRecordTypeId.HasValue || (filter.SummaryItemRecordTypeId.HasValue && filter.SummaryItemRecordTypeId.Value == x.SummaryItemRecordTypeId))
                    ;
                return summaryTransformationDefinitions.FindAllRecords(filterExpression).MapRecords(SummaryTransformationDefinitionInfoMapper);
            }
            else
            {
                return summaryTransformationDefinitions.MapRecords(SummaryTransformationDefinitionInfoMapper);
            }


        }
        public Vanrise.Entities.InsertOperationOutput<SummaryTransformationDefinitionDetail> AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            InsertOperationOutput<SummaryTransformationDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SummaryTransformationDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int summaryTransformationDefinitionId = -1;


            ISummaryTransformationDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddSummaryTransformationDefinition(summaryTransformationDefinition, out summaryTransformationDefinitionId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                summaryTransformationDefinition.SummaryTransformationDefinitionId = summaryTransformationDefinitionId;
                insertOperationOutput.InsertedObject = SummaryTransformationDefinitionDetailMapper(summaryTransformationDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<SummaryTransformationDefinitionDetail> UpdateSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            ISummaryTransformationDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateSummaryTransformationDefinition(summaryTransformationDefinition);
            UpdateOperationOutput<SummaryTransformationDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SummaryTransformationDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SummaryTransformationDefinitionDetailMapper(summaryTransformationDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<SummaryBatchIntervalSettingsConfig> GetSummaryBatchIntervalSourceTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SummaryBatchIntervalSettingsConfig>(SummaryBatchIntervalSettingsConfig.EXTENSION_TYPE);
        }
        #endregion

        #region Private Methods
        private Dictionary<int, SummaryTransformationDefinition> GetCachedSummaryTransformationDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSummaryTransformationDefinitions",
               () =>
               {
                   ISummaryTransformationDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
                   IEnumerable<SummaryTransformationDefinition> summaryTransformationDefinitions = dataManager.GetSummaryTransformationDefinitions();
                   return summaryTransformationDefinitions.ToDictionary(kvp => kvp.SummaryTransformationDefinitionId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISummaryTransformationDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreSummaryTransformationDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        private SummaryTransformationDefinitionDetail SummaryTransformationDefinitionDetailMapper(SummaryTransformationDefinition summaryTransformationDefinitionObject)
        {
            SummaryTransformationDefinitionDetail summaryTransformationDefinitionDetail = new SummaryTransformationDefinitionDetail();
            summaryTransformationDefinitionDetail.Entity = summaryTransformationDefinitionObject;
            return summaryTransformationDefinitionDetail;
        }
        private SummaryTransformationDefinitionInfo SummaryTransformationDefinitionInfoMapper(SummaryTransformationDefinition summaryTransformationDefinitionObject)
        {
            SummaryTransformationDefinitionInfo summaryTransformationDefinitionInfo = new SummaryTransformationDefinitionInfo();
            summaryTransformationDefinitionInfo.SummaryTransformationDefinitionId = summaryTransformationDefinitionObject.SummaryTransformationDefinitionId;
            summaryTransformationDefinitionInfo.Name = summaryTransformationDefinitionObject.Name;
            return summaryTransformationDefinitionInfo;
        }
        #endregion

    }
}
