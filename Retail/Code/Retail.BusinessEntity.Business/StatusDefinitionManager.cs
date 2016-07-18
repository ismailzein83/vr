﻿using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;


namespace Retail.BusinessEntity.Business
{
    public class StatusDefinitionManager
    {

        #region Public Methods

        public StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            Dictionary<Guid, StatusDefinition> cachedStatusDefinitions = this.GetCachedStatusDefinitions();
            return cachedStatusDefinitions.GetRecord(statusDefinitionId);
        }

        public string GetStatusDefinitionName(Guid statusDefinitionId)
        {
            StatusDefinition statusDefinition = this.GetStatusDefinition(statusDefinitionId);
            return (statusDefinition != null) ? statusDefinition.Name : null;
        }

        public IDataRetrievalResult<StatusDefinitionDetail> GetFilteredStatusDefinitions(DataRetrievalInput<StatusDefinitionQuery> input)
        {
            var allStatusDefinitions = GetCachedStatusDefinitions();
            Func<StatusDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStatusDefinitions.ToBigResult(input, filterExpression, StatusDefinitionDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail> AddStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            //int switchId = -1;

            statusDefinitionItem.StatusDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(statusDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                //statusDefinitionItem.SwitchId = switchId;
                insertOperationOutput.InsertedObject = StatusDefinitionDetailMapper(statusDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail> UpdateStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

            if (dataManager.Update(statusDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StatusDefinitionDetailMapper(this.GetStatusDefinition(statusDefinitionItem.StatusDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<StatusDefinitionInfo> GetStatusDefinitionsInfo(StatusDefinitionFilter filter)
        {
            Func<StatusDefinition, bool> filterExpression = null;
            return this.GetCachedStatusDefinitions().MapRecords(StatusDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStatusDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStatusDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, StatusDefinition> GetCachedStatusDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStatusDefinition",
               () =>
               {
                   IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
                   return dataManager.GetStatusDefinition().ToDictionary(x => x.StatusDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        public StatusDefinitionDetail StatusDefinitionDetailMapper(StatusDefinition statusDefinition)
        {
            StatusDefinitionDetail satatusDefinitionDetail = new StatusDefinitionDetail()
            {
                Entity = statusDefinition
            };
            return satatusDefinitionDetail;
        }

        public StatusDefinitionInfo StatusDefinitionInfoMapper(StatusDefinition statusDefinition)
        {
            StatusDefinitionInfo statusDefinitionInfo = new StatusDefinitionInfo()
            {
                StatusDefinitionId = statusDefinition.StatusDefinitionId,
                Name = statusDefinition.Name
            };
            return statusDefinitionInfo;
        }
        #endregion

    }
}
