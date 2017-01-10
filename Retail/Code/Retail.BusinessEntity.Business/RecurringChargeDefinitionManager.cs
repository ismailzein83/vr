using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Data;

namespace Retail.BusinessEntity.Business
{
    public class RecurringChargeDefinitionManager
    {
        #region Public Methods

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

        #endregion
    }
}
