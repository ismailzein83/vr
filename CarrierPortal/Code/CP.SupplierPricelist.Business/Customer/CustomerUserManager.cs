using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using System;

namespace CP.SupplierPricelist.Business
{
    public class CustomerUserManager
    {
        protected CustomerUserDetail MapToDetails(CustomerUser customerUser)
        {
            string username = "";
            User user = new UserManager().GetUserbyId(customerUser.UserId);
            if (user != null)
                username = user.Name;
            return new CustomerUserDetail
            {
                Entity = customerUser,
                UserName = username
            };

        }
        public Vanrise.Entities.IDataRetrievalResult<CustomerUserDetail> GetFilteredCustomerUsers(Vanrise.Entities.DataRetrievalInput<CustomerUserQuery> input)
        {
            var allCustomerSupplierMappings = GetCachedCustomersUsers();
            Func<CustomerUser, bool> filterExpression = (item) => (input.Query.CustomerId == null || input.Query.CustomerId == item.CustomerId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSupplierMappings.ToBigResult(input, filterExpression, MapToDetails));
        }

        public bool GetHasCurrentCustomerId()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var customers = GetCachedCustomersUsers();
            return customers.GetRecord(userId)!=null;
        }


        public int GetCustomerIdByUserId(int userId)
        {
            var customers = GetCachedCustomersUsers();
            return customers.GetRecord(userId).CustomerId;
        }
        public InsertOperationOutput<CustomerUserDetail> AddCustomerUser(CustomerUser input)
        {
            InsertOperationOutput<CustomerUserDetail> insertOperationOutput = new InsertOperationOutput<CustomerUserDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ICustomerUserDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerUserDataManager>();
            bool updateActionSucc = dataManager.Insert(input);
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MapToDetails(input);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
                insertOperationOutput.Message = "This user is already assigned to another customer.";

            }
            return insertOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<CustomerUserDetail> DeleteCustomerUser(int userId)
        {
            ICustomerUserDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerUserDataManager>();
            Vanrise.Entities.DeleteOperationOutput<CustomerUserDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<CustomerUserDetail>();
            bool updateActionSucc = dataManager.Delete(userId);
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            else
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            return deleteOperationOutput;
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
