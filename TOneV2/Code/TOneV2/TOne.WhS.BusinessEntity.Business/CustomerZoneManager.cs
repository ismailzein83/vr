using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneManager
    {
        public Dictionary<int, CustomerZones> GetCachedCustomerZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerZones",
               () =>
               {
                   ICustomerZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
                   IEnumerable<CustomerZones> customerZones = dataManager.GetCustomerZones();
                   return customerZones.ToDictionary(kvp => kvp.CustomerZonesId, kvp => kvp);
               });
        }

        public CustomerZones GetCustomerZone(int customerId, DateTime? effectiveOn, bool futureEntities)
        {
            var cachedCustomerZones = GetCachedCustomerZones();

            if (cachedCustomerZones.Count > 0)
            {
                var filteredZones = cachedCustomerZones.Where(x => x.Value.CustomerId == customerId && x.Value.StartEffectiveTime <= effectiveOn);

                if (filteredZones != null && filteredZones.ToList().Count > 0)
                    return filteredZones.OrderByDescending(x => x.Value.StartEffectiveTime).First().Value;
            }

            return null;
        }

        #region Temp Methods

        public CustomerZones GetCustomerZones(int customerId, DateTime? effectiveOn, bool futureEntities)
        {
            throw new NotImplementedException();
        }

        #endregion

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomerZonesUpdated(ref _updateHandle);
            }
        }
    }
}
