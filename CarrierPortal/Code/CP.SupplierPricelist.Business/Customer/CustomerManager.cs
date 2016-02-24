using System.Collections.Generic;
using System.Linq;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace CP.SupplierPricelist.Business
{
    public class CustomerManager
    {
        protected CustomerDetail MapToDetails(Customer customer)
        {
            string configName = "";
            TemplateConfig config = new TemplateConfigManager().GetTemplateConfiguration(customer.Settings.PriceListConnector.ConfigId);
            if (config != null)
                configName = config.Name;
            return new CustomerDetail
            {
                Entity = customer,
                ConfigName = configName
            };

        }
        public IDataRetrievalResult<CustomerDetail> GetFilteredCustomers(DataRetrievalInput<Customer> input)
        {
            return DataRetrievalManager.Instance.ProcessResult(input, GetCachedCustomers().ToBigResult(input, null, MapToDetails));
        }
        public InsertOperationOutput<CustomerDetail> AddCustomer(Customer inputCustomer)
        {
            InsertOperationOutput<CustomerDetail> insertOperationOutput = new InsertOperationOutput<CustomerDetail>();
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
            return updateOperationOutput;
        }
        public UpdateOperationOutput<CustomerDetail> AddUser(Customer input)
        {
            UpdateOperationOutput<CustomerDetail> updateOperationOutput = new UpdateOperationOutput<CustomerDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            ICustomerDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerDataManager>();
            bool updateActionSucc = dataManager.AddUser(input);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(input);
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
        public List<TemplateConfig> GetConnectorTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerConnector);
        }
        #endregion


    }
}
