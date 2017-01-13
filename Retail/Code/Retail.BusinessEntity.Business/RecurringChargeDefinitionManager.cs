using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Data;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RecurringChargeDefinitionManager
    {
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<RecurringChargeDefinitionDetail> GetFilteredRecurringChargeDefinitions(Vanrise.Entities.DataRetrievalInput<RecurringChargeDefinitionQuery> input)
        {
            var allRecurringChargeDefinitions = GetCachedRecurringChargeDefinitions();
            Func<RecurringChargeDefinition, bool> filterExpression = (RecurringChargeDefinition) => (input.Query.Name == null || RecurringChargeDefinition.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRecurringChargeDefinitions.ToBigResult(input, filterExpression, RecurringChargeDefinitionDetailMapper));
        }

        public RecurringChargeDefinition GetRecurringChargeDefinition(Guid RecurringChargeDefinitionId)
        {
            return this.GetCachedRecurringChargeDefinitions().GetRecord(RecurringChargeDefinitionId);
        }

        public IEnumerable<RecurringChargeDefinitionInfo> GetRecurringChargeDefinitionsInfo(RecurringChargeDefinitionInfoFilter filter)
        {
            Func<RecurringChargeDefinition, bool> filterExpression = null;
            //if (filter != null)
            //{
            //    filterExpression = (item) =>
            //    {
            //        if (filter.EntityType == null || item.EntityType == filter.EntityType)
            //            return true;
            //        return false;
            //    };
            //}
            return this.GetCachedRecurringChargeDefinitions().MapRecords(RecurringChargeDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.InsertOperationOutput<RecurringChargeDefinitionDetail> AddRecurringChargeDefinition(RecurringChargeDefinition recurringChargeDefinition)
        {
            InsertOperationOutput<RecurringChargeDefinitionDetail> insertOperationOutput = new InsertOperationOutput<RecurringChargeDefinitionDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            recurringChargeDefinition.RecurringChargeDefinitionId = Guid.NewGuid();
            IRecurringChargeDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurringChargeDefinitionDataManager>();

            bool insertActionSucc = dataManager.Insert(recurringChargeDefinition);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RecurringChargeDefinitionDetailMapper(recurringChargeDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<RecurringChargeDefinitionDetail> UpdateRecurringChargeDefinition(RecurringChargeDefinition recurringChargeDefinition)
        {
            IRecurringChargeDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurringChargeDefinitionDataManager>();

            bool updateActionSucc = dataManager.Update(recurringChargeDefinition);
            UpdateOperationOutput<RecurringChargeDefinitionDetail> updateOperationOutput = new UpdateOperationOutput<RecurringChargeDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RecurringChargeDefinitionDetailMapper(recurringChargeDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRecurringChargeDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IRecurringChargeDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRecurringChargeDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, RecurringChargeDefinition> GetCachedRecurringChargeDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRecurringChargeDefinitions",
               () =>
               {
                   IRecurringChargeDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurringChargeDefinitionDataManager>();
                   return dataManager.GetRecurringChargeDefinitions().ToDictionary(x => x.RecurringChargeDefinitionId, x => x);
               });
        }

        #endregion

        #region Mappers

        public RecurringChargeDefinitionInfo RecurringChargeDefinitionInfoMapper(RecurringChargeDefinition recurringChargeDefinition)
        {
            RecurringChargeDefinitionInfo recurringChargeDefinitionInfo = new RecurringChargeDefinitionInfo()
            {
                RecurringChargeDefinitionId = recurringChargeDefinition.RecurringChargeDefinitionId,
                Name = recurringChargeDefinition.Name
            };
            return recurringChargeDefinitionInfo;
        }

        public RecurringChargeDefinitionDetail RecurringChargeDefinitionDetailMapper(RecurringChargeDefinition recurringChargeDefinition)
        {
            RecurringChargeDefinitionDetail recurringChargeDefinitionDetail = new RecurringChargeDefinitionDetail()
            {
                Entity = recurringChargeDefinition
            };
            return recurringChargeDefinitionDetail;
        }

        #endregion
    }
}
