using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;


namespace CP.SupplierPricelist.Business
{
    public class CustomerSupplierMappingManager
    {
        public List<SupplierInfo> GetCustomerSuppliers()
        {
            int customerId = GetLoggedInCustomerId();
            Customer customer = new CustomerManager().GetCustomer(customerId);
            if (customer != null)
            {
                SupplierPriceListConnectorBase supplierPriceListConnectorBase = customer.Settings.PriceListConnector as SupplierPriceListConnectorBase;

                return supplierPriceListConnectorBase.GetSuppliers(null);
            }
            else 
                throw new NotSupportedException();          
            
        }

        public Vanrise.Entities.IDataRetrievalResult<CustomerSupplierMapping> GetFilteredCustomerSupplierMappings(Vanrise.Entities.DataRetrievalInput<CustomerSupplierMappingQuery> input)
        {
            var allCustomerSupplierMappings = GetCachedCustomerSupplierMappingsUsers();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSupplierMappings.ToBigResult(input, null));
        }

        public IEnumerable<CustomerSupplierMapping> GetCustomerSupplierMappings()
        {
            var customerSupplierMappings = GetCachedCustomerSupplierMappingsUsers();
            return customerSupplierMappings.Values;
        }
        public Vanrise.Entities.InsertOperationOutput<CustomerSupplierMapping> AddCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            Vanrise.Entities.InsertOperationOutput<CustomerSupplierMapping> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CustomerSupplierMapping>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            customerSupplierMapping.CustomerId = GetLoggedInCustomerId();
            ICustomerSupplierMappingDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerSupplierMappingDataManager>();
            bool insertActionSucc = dataManager.Insert(customerSupplierMapping);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
               
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = customerSupplierMapping;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;


            return insertOperationOutput;
        }
        #region Private Methods
        int GetLoggedInCustomerId()
        {
            int userId = new SecurityContext().GetLoggedInUserId();
            return new CustomerUserManager().GetCustomerIdByUserId(userId);
        }

        Dictionary<int, CustomerSupplierMapping> GetCachedCustomerSupplierMappingsUsers()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomersSupplierMappings",
               () =>
               {
                   ICustomerSupplierMappingDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerSupplierMappingDataManager>();
                   IEnumerable<CustomerSupplierMapping> customerSupplierMappings = dataManager.GetAllCustomerSupplierMappings();
                   return customerSupplierMappings.ToDictionary(cu => cu.UserId, cu => cu);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerSupplierMappingDataManager _dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerSupplierMappingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomerSupplierMappingsUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
