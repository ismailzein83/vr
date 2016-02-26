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
        public IEnumerable<SupplierInfo> GetCustomerSuppliers(SupplierInfoFilter filter)
        {
            int customerId = GetLoggedInCustomerId();
            Customer customer = new CustomerManager().GetCustomer(customerId);
            if (customer != null)
            {
                SupplierPriceListConnectorBase supplierPriceListConnectorBase = customer.Settings.PriceListConnector as SupplierPriceListConnectorBase;

                var allCustomerSuppliers = supplierPriceListConnectorBase.GetSuppliers(null);
                if (allCustomerSuppliers == null)
                    return null;
                else
                {
                    var allSupplierMappings = GetCachedCustomerSupplierMappings();
                    var currentCustomerSupplierMappings = allSupplierMappings.FindAllRecords(itm => itm.Value.CustomerId == customerId);
                    Func<SupplierInfo, bool> filterExpression = (supplierInfo) =>
                        {
                            if(filter.AssignableToSupplierUserId.HasValue)
                            {
                                if (currentCustomerSupplierMappings.Any(itm => 
                                    itm.Value.UserId != filter.AssignableToSupplierUserId.Value 
                                    && itm.Value.Settings.MappedSuppliers.Any(sup => sup == supplierInfo.SupplierId)))
                                    return false;
                            }
                            return true;
                        };
                    return allCustomerSuppliers.FindAllRecords(filterExpression);
                }
            }
            else
                throw new NotSupportedException();

        }

        

        public Vanrise.Entities.IDataRetrievalResult<CustomerSupplierMappingDetail> GetFilteredCustomerSupplierMappings(Vanrise.Entities.DataRetrievalInput<CustomerSupplierMappingQuery> input)
        {
            var allCustomerSupplierMappings = GetCachedCustomerSupplierMappings();
            Func<CustomerSupplierMapping, bool> filterExpression = (item) =>
                 (input.Query.Users == null || input.Query.Users.Count() == 0 || input.Query.Users.Contains(item.UserId))
                  &&
                  (item.CustomerId == GetLoggedInCustomerId())
                  &&
                 (input.Query.CarrierAccouts == null || input.Query.CarrierAccouts.Count() == 0 || input.Query.CarrierAccouts.Any(y => (item.Settings.MappedSuppliers.Any(x => x == y))));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSupplierMappings.ToBigResult(input, filterExpression, SupplierMappingDetailMapper));
        }

        public CustomerSupplierMapping GetCustomerSupplierMapping(int userId)
        {
            var customerSupplierMappings = GetCachedCustomerSupplierMappings();
            return customerSupplierMappings.Values.FindRecord(x => x.UserId == userId && x.CustomerId == GetLoggedInCustomerId());
        }

        public IEnumerable<CustomerSupplierMapping> GetCustomerSupplierMappings()
        {
            var customerSupplierMappings = GetCachedCustomerSupplierMappings();
            return customerSupplierMappings.Values;
        }
        public Vanrise.Entities.InsertOperationOutput<CustomerSupplierMappingDetail> AddCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            Vanrise.Entities.InsertOperationOutput<CustomerSupplierMappingDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CustomerSupplierMappingDetail>();

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
                insertOperationOutput.InsertedObject = SupplierMappingDetailMapper(customerSupplierMapping);
            }
           

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<CustomerSupplierMappingDetail> UpdateCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            Vanrise.Entities.UpdateOperationOutput<CustomerSupplierMappingDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CustomerSupplierMappingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            customerSupplierMapping.CustomerId = GetLoggedInCustomerId();
            ICustomerSupplierMappingDataManager dataManager = CustomerDataManagerFactory.GetDataManager<ICustomerSupplierMappingDataManager>();
            bool updateActionSucc = dataManager.Update(customerSupplierMapping);
            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SupplierMappingDetailMapper(customerSupplierMapping);
            }


            return updateOperationOutput;
        }
        #region Private Methods

        private List<SupplierInfo> GetAllCustomerSuppliersByLoggedInUser() 
        {
        
            int customerId = GetLoggedInCustomerId();
            Customer customer = new CustomerManager().GetCustomer(customerId);
            if (customer != null)
            {
                SupplierPriceListConnectorBase supplierPriceListConnectorBase = customer.Settings.PriceListConnector as SupplierPriceListConnectorBase;

                return supplierPriceListConnectorBase.GetSuppliers(null);
            }
            else
               return null;
        
        }
        public int GetLoggedInCustomerId()
        {
            int userId = new SecurityContext().GetLoggedInUserId();
            return new CustomerUserManager().GetCustomerIdByUserId(userId);
        }

        public List<int> GetCustomersIdsByLoggedInUserId()
        {
            int userId = new SecurityContext().GetLoggedInUserId();
            return GetCachedCustomerSupplierMappings().Values.FindAllRecords(x => x.UserId == userId).Select(c => c.CustomerId).ToList();
        }


        Dictionary<int, CustomerSupplierMapping> GetCachedCustomerSupplierMappings()
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
                customerSupplierMappingDetail.SupplierNames = string.Join(",", GetAllCustomerSuppliersByLoggedInUser().Where(u=> customerSupplierMapping.Settings.MappedSuppliers.Contains(u.SupplierId)).Select(x => x.SupplierName));
            }

            User user = new UserManager().GetUserbyId(customerSupplierMapping.UserId);
            if (user != null)
                customerSupplierMappingDetail.UserName = user.Name;

            return customerSupplierMappingDetail;
        }
    }
}
