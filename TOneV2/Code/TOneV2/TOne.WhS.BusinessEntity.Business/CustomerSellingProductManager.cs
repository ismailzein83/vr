using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerSellingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerSellingProductDetail> GetFilteredCustomerSellingProducts(Vanrise.Entities.DataRetrievalInput<CustomerSellingProductQuery> input)
        {

            var allCustomerSellingProducts = GetCachedCustomerSellingProducts();

            Func<CustomerSellingProduct, bool> filterExpression = (prod) =>
                 (input.Query.EffectiveDate == null || (prod.BED<input.Query.EffectiveDate))
                 &&
                 (input.Query.CustomersIds == null || input.Query.CustomersIds.Contains(prod.CustomerId))
                  &&
                 (input.Query.SellingProductsIds == null || input.Query.SellingProductsIds.Contains(prod.SellingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSellingProducts.ToBigResult(input, filterExpression,CustomerSellingProductDetailMapper));
        }

        public CustomerSellingProduct GetCustomerSellingProduct(int customerSellingProductId)
        {
            var customerSellingProducts = GetCachedCustomerSellingProducts();
            return customerSellingProducts.GetRecord(customerSellingProductId);
        }

        public CustomerSellingProduct GetEffectiveSellingProduct(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            string cacheName = String.Format("GetEffectiveSellingProduct_{0}_{1}_{2}", customerId, effectiveOn.HasValue ? effectiveOn.Value.Date : default(DateTime), isEffectiveInFuture);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
              () =>
              {
                  var orderedCustomerSellingProducts = GetCachedCustomerSellingProducts().Values.OrderByDescending(x=>x.BED);
                 
                  CustomerSellingProduct customerSellingProduct = orderedCustomerSellingProducts.FirstOrDefault(itm => itm.CustomerId == customerId && ((effectiveOn.HasValue && effectiveOn.Value >= itm.BED) || isEffectiveInFuture));
                  if (customerSellingProduct != null)
                  {
                      return new CustomerSellingProduct
                      {
                          SellingProductId = customerSellingProduct.SellingProductId,
                          CustomerSellingProductId = customerSellingProduct.CustomerSellingProductId,
                          CustomerId = customerSellingProduct.CustomerId,
                          BED = customerSellingProduct.BED
                      };
                  }
                  else
                      return null;
              });
        }


        public TOne.Entities.InsertOperationOutput<List<CustomerSellingProductDetail>> AddCustomerSellingProduct(List<CustomerSellingProduct> customerSellingProducts)
        {

            TOne.Entities.InsertOperationOutput<List<CustomerSellingProductDetail>> insertOperationOutput = new TOne.Entities.InsertOperationOutput<List<CustomerSellingProductDetail>>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            foreach(var obj in customerSellingProducts)
            {
                var effectiveCustomerSellingProduct = GetEffectiveSellingProduct(obj.CustomerId, DateTime.Now, false);

                if (effectiveCustomerSellingProduct != null && (obj.BED <= effectiveCustomerSellingProduct.BED || obj.SellingProductId == effectiveCustomerSellingProduct.SellingProductId))
                {
                    return insertOperationOutput;
                }
            }

            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            List<CustomerSellingProduct> customerSellingProductObject = new List<CustomerSellingProduct>();

            foreach (CustomerSellingProduct customerSellingProduct in customerSellingProducts)
                {
                    customerSellingProductObject.Add(new CustomerSellingProduct
                   {
                       CustomerId = customerSellingProduct.CustomerId,
                       BED = customerSellingProduct.BED,
                       SellingProductId = customerSellingProduct.SellingProductId,
                      });
                 }
            List<CustomerSellingProduct> insertedObjects = new List<CustomerSellingProduct>();

            List<CustomerSellingProductDetail> returnedData = new List<CustomerSellingProductDetail>();
            bool insertActionSucc = dataManager.Insert(customerSellingProductObject, out  insertedObjects);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                foreach (CustomerSellingProduct customerSellingProduct in insertedObjects)
                {
                    var obj = CustomerSellingProductDetailMapper(customerSellingProduct);
                    returnedData.Add(new CustomerSellingProductDetail
                    {
                        Entity = obj.Entity,
                        CustomerName = obj.CustomerName,
                        SellingProductName = obj.SellingProductName
                    });
                }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = returnedData;
            }
           
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail> UpdateCustomerSellingProduct(CustomerSellingProduct customerSellingProduct)
        {
            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
 
            var effectiveCustomerSellingProduct = GetEffectiveSellingProduct(customerSellingProduct.CustomerId, DateTime.Now, false);

            if (customerSellingProduct.BED < DateTime.Now || (effectiveCustomerSellingProduct != null &&  customerSellingProduct.BED < effectiveCustomerSellingProduct.BED))
            {
                return updateOperationOutput;
            }    
            bool updateActionSucc = dataManager.Update(customerSellingProduct);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                CustomerSellingProductDetail customerSellingProductDetail = CustomerSellingProductDetailMapper(customerSellingProduct);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = customerSellingProductDetail;
            }

            return updateOperationOutput;
        }

        public IEnumerable<CustomerSellingProduct> GetEffectiveInFutureCustomerSellingProduct()
        {
            var customerSellingProducts = GetCachedCustomerSellingProducts();
           
            return customerSellingProducts.Values.FindAllRecords(x => x.BED > DateTime.Now);
        }

        public bool IsAssignableCustomerToSellingProduct(int customerId)
        {
            var customerSellingProducts = GetCachedCustomerSellingProducts();
            return customerSellingProducts.Values.Any(x => x.BED > DateTime.Now && x.CustomerId == customerId);


        }

        public int? GetEffectiveSellingProductId(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            int? sellingProductId = null;
            CustomerSellingProduct customerSellingProduct = GetEffectiveSellingProduct(customerId, effectiveOn, isEffectiveInFuture);
            
            if (customerSellingProduct != null)
                sellingProductId = customerSellingProduct.SellingProductId;

            return sellingProductId;
        }

        #region Private Members

        Dictionary<int,CustomerSellingProduct> GetCachedCustomerSellingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedOrderedCustomerSellingProducts",
               () =>
               {
                   ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
                   IEnumerable<CustomerSellingProduct> customerSellingProducts = dataManager.GetCustomerSellingProducts();
                   return customerSellingProducts.ToDictionary(kvp => kvp.CustomerSellingProductId, kvp => kvp);
               });
        }
        private CustomerSellingProductDetail CustomerSellingProductDetailMapper(CustomerSellingProduct customerSellingProduct)
        {
            CustomerSellingProductDetail customerSellingProductDetail = new CustomerSellingProductDetail();

            customerSellingProductDetail.Entity = customerSellingProduct;

            SellingProductManager sellingProductManager = new SellingProductManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SellingProduct sellingProduct = sellingProductManager.GetSellingProduct(customerSellingProduct.SellingProductId);
            customerSellingProductDetail.CustomerName = carrierAccountManager.GetCarrierAccountName(customerSellingProduct.CustomerId);
            if (sellingProduct != null)
            {
                customerSellingProductDetail.SellingProductName = sellingProduct.Name;
            }
            return customerSellingProductDetail;
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerSellingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomerSellingProductsUpdated(ref _updateHandle);
            }
        }


        #endregion
    }


}
