using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRecurringChargeTypeManager
    {
        public static Guid supplierRecurringChargesTypeBEDefinitionId = new Guid("7501c2b2-374b-432c-b423-59e772663eb8");
        
        #region Public Mehods
        public string GetSupplierRecurringChargeTypeName(long supplierRecurringChargesTypeId)
        {
            Dictionary<long, SupplierRecurringChargesType> cachedEntities = this.GetAllCachedSupplierRecurringChargesTypes();
            Func<SupplierRecurringChargesType, bool> filterFunc = (entity) =>
           {
               if (entity.SupplierRecurringChargeTypeId != supplierRecurringChargesTypeId)
                   return false;
               return true;
           };
            IEnumerable<SupplierRecurringChargesType> filteredEntities = cachedEntities.FindAllRecords(filterFunc).OrderByDescending(x => x.SupplierRecurringChargeTypeId);
            return filteredEntities.FirstOrDefault().Name;
        }
        #endregion

        #region Private Methods
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierRecurringChargesTypeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierRecurringChargesTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllSupplierRecurringChargesTypesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<long, SupplierRecurringChargesType> GetAllCachedSupplierRecurringChargesTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllSupplierRecurringChargesTypes",
               () =>
               {
                   ISupplierRecurringChargesTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRecurringChargesTypeDataManager>();
                   IEnumerable<SupplierRecurringChargesType> supplierRecurringChargesTypes = dataManager.GetAllSupplierRecurringChargesTypes();
                   return supplierRecurringChargesTypes.ToDictionary(record => record.SupplierRecurringChargeTypeId, record => record);
               });
        }

        #endregion

    }
}
