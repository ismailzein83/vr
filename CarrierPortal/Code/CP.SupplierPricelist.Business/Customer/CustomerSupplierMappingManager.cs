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
using Vanrise.Security.Entities;


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

        public Vanrise.Entities.IDataRetrievalResult<CustomerSupplierMappingDetail> GetFilteredCustomerSupplierMappings(Vanrise.Entities.DataRetrievalInput<CustomerSupplierMappingQuery> input)
        {
            var allCustomerSupplierMappings = GetCachedCustomerSupplierMappingsUsers();
            Func<CustomerSupplierMapping, bool> filterExpression = (item) =>
                 (input.Query.Users == null || input.Query.Users.Count() == 0 || input.Query.Users.Contains(item.UserId))
                  &&
                 (input.Query.CarrierAccouts == null || input.Query.CarrierAccouts.Count() == 0 || input.Query.CarrierAccouts.Any(y => (item.Settings.MappedSuppliers.Any(x => x.SupplierId == y))));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSupplierMappings.ToBigResult(input, filterExpression, SupplierMappingDetailMapper));
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
            int supplierMappingId = -1;

            ICustomerSupplierMappingDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerSupplierMappingDataManager>();
            bool insertActionSucc = dataManager.Insert(customerSupplierMapping, out supplierMappingId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                customerSupplierMapping.SupplierMappingId = supplierMappingId;
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
                   return customerSupplierMappings.ToDictionary(cu => cu.SupplierMappingId, cu => cu);
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

        protected CustomerSupplierMappingDetail SupplierMappingDetailMapper(CustomerSupplierMapping customerSupplierMapping)
        {
            CustomerSupplierMappingDetail customerSupplierMappingDetail = new CustomerSupplierMappingDetail()
            {
                Entity = customerSupplierMapping
            };

            if (customerSupplierMapping.Settings.MappedSuppliers.Count() > 0)
            {
                customerSupplierMappingDetail.SupplierNames = string.Join(",", customerSupplierMapping.Settings.MappedSuppliers.Select(x => x.SupplierName)); ;
            }

            User user = new UserManager().GetUserbyId(customerSupplierMapping.UserId);
            if (user != null)
                customerSupplierMappingDetail.UserName = user.Name;

            return customerSupplierMappingDetail;
        }
    }
}
