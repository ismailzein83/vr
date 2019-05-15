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
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(itemObject.DevProjectId))
                    return false;

                if (input.Query.Name != null && !itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };

            VRActionLogger.Current.LogGetFilteredAction(SummaryTransformationDefinitionLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, SummaryTransformationDefinitionDetailMapper));
        }
        public SummaryTransformationDefinition GetSummaryTransformationDefinition(Guid summaryTransformationDefinitionId)
        {
            var summaryTransformationDefinitions = GetCachedSummaryTransformationDefinitions();
            return summaryTransformationDefinitions.GetRecord(summaryTransformationDefinitionId);
        }
        public IEnumerable<SummaryTransformationDefinitionInfo> GetSummaryTransformationDefinitionInfo(SummaryTransformationDefinitionInfoFilter filter)
        {
            var summaryTransformationDefinitions = GetCachedSummaryTransformationDefinitions();

            Func<SummaryTransformationDefinition, bool> filterExpression = (x) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(x.DevProjectId))
                    return false;

                if (filter != null)
                {
                    if (filter.RawItemRecordTypeId.HasValue && filter.RawItemRecordTypeId.Value != x.RawItemRecordTypeId)
                        return false;
                    if (filter.SummaryItemRecordTypeId.HasValue && filter.SummaryItemRecordTypeId.Value != x.SummaryItemRecordTypeId)
                        return false;
                }

                return true;
            };
            return summaryTransformationDefinitions.FindAllRecords(filterExpression).MapRecords(SummaryTransformationDefinitionInfoMapper);
        }
        public Vanrise.Entities.InsertOperationOutput<SummaryTransformationDefinitionDetail> AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            InsertOperationOutput<SummaryTransformationDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SummaryTransformationDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            summaryTransformationDefinition.SummaryTransformationDefinitionId = Guid.NewGuid();


            ISummaryTransformationDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<ISummaryTransformationDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddSummaryTransformationDefinition(summaryTransformationDefinition);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(SummaryTransformationDefinitionLoggableEntity.Instance, summaryTransformationDefinition);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SummaryTransformationDefinitionDetailMapper(summaryTransformationDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public string GetSummaryTransformationDefinitionName(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            if (summaryTransformationDefinition != null)
               return summaryTransformationDefinition.Name;

            return null;
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(SummaryTransformationDefinitionLoggableEntity.Instance, summaryTransformationDefinition);
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
        private Dictionary<Guid, SummaryTransformationDefinition> GetCachedSummaryTransformationDefinitions()
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

        private class SummaryTransformationDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static SummaryTransformationDefinitionLoggableEntity Instance = new SummaryTransformationDefinitionLoggableEntity();

            private SummaryTransformationDefinitionLoggableEntity()
            {

            }

            static SummaryTransformationDefinitionManager s_summaryTransformationDefinitionManager = new SummaryTransformationDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_SummaryTransformationDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Summary Transformation Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_SummaryTransformationDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SummaryTransformationDefinition summaryTransformationDefinition = context.Object.CastWithValidate<SummaryTransformationDefinition>("context.Object");
                return summaryTransformationDefinition.SummaryTransformationDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SummaryTransformationDefinition summaryTransformationDefinition = context.Object.CastWithValidate<SummaryTransformationDefinition>("context.Object");
                return s_summaryTransformationDefinitionManager.GetSummaryTransformationDefinitionName(summaryTransformationDefinition);
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
