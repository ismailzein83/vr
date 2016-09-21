using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using System;

namespace CP.SupplierPricelist.Business
{
    public class CustomerManager
    {
        protected CustomerDetail MapToDetails(Customer customer)
        {
            string configName = "";
            IEnumerable<CustomerConnectorConfig> configs = new ExtensionConfigurationManager().GetExtensionConfigurations<CustomerConnectorConfig>(CustomerConnectorConfig.EXTENSION_TYPE);
              
            CustomerConnectorConfig config = null;
            if(configs != null)
            {
                config = configs.FindRecord(x => x.ExtensionConfigurationId == customer.Settings.PriceListConnector.ConfigId);
            }

            if (config != null)
                configName = config.Name;
            return new CustomerDetail
            {
                Entity = customer,
                ConfigName = configName
            };

        }
        protected CustomerInfo CustomerInfoMapper(Customer customer)
        {

            return new CustomerInfo
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name
            };

        }

        public IEnumerable<SupplierInfo> GetCustomerSuppliers(int customerId)
        {
            return GetCachedSupplierAccounts(customerId).Values;
        }

        public IEnumerable<CustomerInfo> GetCustomerInfos(CustomerFilter filter)
        {
            var cachedCustomers = GetCachedCustomers();
            if (filter != null && filter.AssignedToCurrentSupplier == true)
            {
                CustomerSupplierMappingManager csmanger = new CustomerSupplierMappingManager();
                return cachedCustomers.MapRecords(CustomerInfoMapper, cus => (csmanger.GetCustomersIdsByLoggedInUserId().Contains(cus.CustomerId)));
            }
            return cachedCustomers.MapRecords(CustomerInfoMapper);

        }
        public IDataRetrievalResult<CustomerDetail> GetFilteredCustomers(DataRetrievalInput<CustomerQuery> input)
        {
            var cachedCustomers = GetCachedCustomers();
            Func<Customer, bool> filterExpression = item =>
                   (string.IsNullOrEmpty(input.Query.Name) || item.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedCustomers.ToBigResult(input, filterExpression, MapToDetails));
        }
        public InsertOperationOutput<CustomerDetail> AddCustomer(Customer inputCustomer)
        {
            InsertOperationOutput<CustomerDetail> insertOperationOutput = new InsertOperationOutput<CustomerDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            int customerId;
            ICustomerDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            bool insertActionSucc = dataManager.AddCustomer(inputCustomer, out customerId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                inputCustomer.CustomerId = customerId;
                insertOperationOutput.InsertedObject = MapToDetails(inputCustomer);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<CustomerDetail> UpdateCustomer(Customer input)
        {
            UpdateOperationOutput<CustomerDetail> updateOperationOutput = new UpdateOperationOutput<CustomerDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            ICustomerDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            bool updateActionSucc = dataManager.UpdateCustomer(input);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(input);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
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

        public Dictionary<string, SupplierInfo> GetCachedSupplierAccounts(int customerId)
        {
            string cacheName = String.Format("CustomerManager_GetCachedSupplierAccounts_{0}", customerId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   Customer customer = GetCustomer(customerId);
                   if (customer == null)
                       throw new NullReferenceException("customer");
                   if (customer.Settings == null)
                       throw new NullReferenceException("customer.Settings");
                   if (customer.Settings.PriceListConnector == null)
                       throw new NullReferenceException("customer.Settings.PriceListConnector");

                   IEnumerable<SupplierInfo> supplierAccounts = customer.Settings.PriceListConnector.GetSuppliers(null);
                   return supplierAccounts.ToDictionary(c => c.SupplierId, c => c);
               });
        }
        private class SupplierCacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get
                {
                    return true;
                }
            }

        }

        public IEnumerable<CustomerConnectorConfig> GetConnectorTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CustomerConnectorConfig>(CustomerConnectorConfig.EXTENSION_TYPE);
        }
        #endregion


    }
}
