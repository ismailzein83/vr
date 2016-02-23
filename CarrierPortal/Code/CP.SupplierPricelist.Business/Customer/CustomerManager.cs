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
            return new CustomerDetail
            {
                Entity = customer
            };
        }
        public IDataRetrievalResult<CustomerDetail> GetFilteredCustomers(DataRetrievalInput<Customer> input)
        {
            return DataRetrievalManager.Instance.ProcessResult(input, GetCachedCustomers().ToBigResult(input, null, MapToDetails));
        }
        public InsertOperationOutput<Customer> AddCustomer(Customer inputCustomer)
        {
            InsertOperationOutput<Customer> insertOperationOutput = new InsertOperationOutput<Customer>();
            int customerId = -1;
            ICustomerDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            bool insertActionSucc = dataManager.AddCustomer(inputCustomer, out customerId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                inputCustomer.CustomerId = customerId;
                insertOperationOutput.InsertedObject = inputCustomer;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
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
        public List<Vanrise.Entities.TemplateConfig> GetConnectorTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerConnector);
        }
        #endregion


    }
}
