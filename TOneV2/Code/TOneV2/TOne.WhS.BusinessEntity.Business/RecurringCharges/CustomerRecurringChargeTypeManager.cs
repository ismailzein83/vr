using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RecurringCharges
{
    public class CustomerRecurringChargeTypeManager
    {
        public static Guid customerRecurringChargesTypeBEDefinitionId = new Guid("61885a4a-647d-45ea-810a-7028ae9a7f1f");

        #region Public Mehods
        public string GetCustomerRecurringChargeTypeName(long customerRecurringChargeTypeId)
        {
            Dictionary<long, CustomerRecurringChargesType> cachedEntities = this.GetAllCachedCustomerRecurringChargesTypes();
            return cachedEntities[customerRecurringChargeTypeId].Name;
        }
        #endregion

        #region Private Methods
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerRecurringChargesTypeDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerRecurringChargesTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllCustomerRecurringChargesTypesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<long, CustomerRecurringChargesType> GetAllCachedCustomerRecurringChargesTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerRecurringChargesTypes",
               () =>
               {
                   ICustomerRecurringChargesTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerRecurringChargesTypeDataManager>();
                   IEnumerable<CustomerRecurringChargesType> customerRecurringChargesTypes = dataManager.GetAllCustomerRecurringChargesTypes();
                   return customerRecurringChargesTypes.ToDictionary(record => record.CustomerRecurringChargeTypeId, record => record);
               });
        }

        #endregion
    }
}
