using System;
using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace CP.SupplierPricelist.Business
{
    public class CustomerManager
    {
        public CustomerOutput GetCustomers(ref byte[] maxTimeStamp, int nbOfRows)
        {
            CustomerOutput output = new CustomerOutput();
            ICustomerDataManager dataManager =
             CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            List<Customer> customers = dataManager.GetCustomers(ref maxTimeStamp, nbOfRows);
            output.Customers = customers;
            output.MaxTimeStamp = maxTimeStamp;
            return output;
        }

        public Customer GetCustomer(int customerId)
        {
            var customers = GetCachedCustomers();
            return customers.GetRecord(customerId);
        }

        #region Private Methods
        Dictionary<int, Customer> GetCachedCustomers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomers",
               () =>
               {
                   ICustomerDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
                   IEnumerable<Customer> customers = dataManager.GetAllCustomers();
                   return customers.ToDictionary(c => c.CustomerId, c => c);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerDataManager _dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomersUpdated(ref _updateHandle);
            }
        }


        #endregion


    }
}
