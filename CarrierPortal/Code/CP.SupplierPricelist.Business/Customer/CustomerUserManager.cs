using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Business
{
    public class CustomerUserManager
    {

        public int GetCustomerIdByUserId(int customerId)
        {
            var customers = GetCachedCustomersUsers();
            return customers.GetRecord(customerId).CustomerId;
        }
        public UpdateOperationOutput<CustomerUser> AddUser(CustomerUser input)
        {
            UpdateOperationOutput<CustomerUser> updateOperationOutput = new UpdateOperationOutput<CustomerUser>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            ICustomerUserDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerUserDataManager>();
            bool updateActionSucc = dataManager.AddUser(input);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = input;
            }
            return updateOperationOutput;
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
