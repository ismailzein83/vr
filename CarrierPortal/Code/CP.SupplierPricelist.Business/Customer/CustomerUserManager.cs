using System;
using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace CP.SupplierPricelist.Business
{
    public class CustomerUserManager
    {

        public int GetCustomerIdByUserId(int customerId)
        {
            var customers = GetCachedCustomersUsers();
            return customers.GetRecord(customerId).CustomerId;
        }

        #region Private Methods
        Dictionary<int, CustomerUser> GetCachedCustomersUsers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomers",
               () =>
               {
                   ICustomerUserDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerUserDataManager>();
                   IEnumerable<CustomerUser> customers = dataManager.GetAllCustomersUsers();
                   return customers.ToDictionary(cu => cu.UserId, cu => cu);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerUserDataManager _dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerUserDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomersUsersUpdated(ref _updateHandle);
            }
        }

    }
    #endregion
}
