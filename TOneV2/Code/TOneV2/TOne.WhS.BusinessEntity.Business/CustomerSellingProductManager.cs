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

            var allCustomerSellingProducts = GetCachedOrderedCustomerSellingProducts();

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
            var customerSellingProducts = GetCachedOrderedCustomerSellingProducts();
            return customerSellingProducts.GetRecord(customerSellingProductId);
        }

        public CustomerSellingProduct GetEffectiveSellingProduct(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            string cacheName = String.Format("GetEffectiveSellingProduct_{0}_{1}_{2}", customerId, effectiveOn.HasValue ? effectiveOn.Value.Date : default(DateTime), isEffectiveInFuture);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
              () =>
              {
                  var orderedCustomerSellingProducts = GetCachedOrderedCustomerSellingProducts();
                 
                  CustomerSellingProduct customerSellingProduct = orderedCustomerSellingProducts.FirstOrDefault(itm => itm.Value.CustomerId == customerId && ((effectiveOn.HasValue && effectiveOn.Value >= itm.Value.BED) || isEffectiveInFuture)).Value;
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


        public TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>> AddCustomerSellingProduct(List<CustomerSellingProduct> customerSellingProducts)
        {

            TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>> insertOperationOutput = new TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>>();

            //insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            //insertOperationOutput.InsertedObject = null;

          //  int customerSellingProductId = -1;
            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            List<CustomerSellingProduct> customerSellingProductObject = new List<CustomerSellingProduct>();

            #region Insertion Rules
            foreach (CustomerSellingProduct customerSellingProduct in customerSellingProducts)
                {
                    var cashedCustomerSellingProducts = GetCachedOrderedCustomerSellingProducts();


                    IEnumerable<CustomerSellingProduct> customerSellingProductList = cashedCustomerSellingProducts.FindAllRecords(x => x.CustomerId == customerSellingProduct.CustomerId);
                    if (customerSellingProductList == null || customerSellingProductList.Count() == 0)
                    {
                        customerSellingProductObject.Add(new CustomerSellingProduct
                        {
                            CustomerId = customerSellingProduct.CustomerId,
                            BED = customerSellingProduct.BED,
                            SellingProductId = customerSellingProduct.SellingProductId,
                        });
                    }
                    else
                    {
                        foreach (CustomerSellingProduct obj in customerSellingProductList)
                        {
                                if (obj.BED < customerSellingProduct.BED)
                                {
                                    customerSellingProductObject.Add(new CustomerSellingProduct
                                    {
                                        CustomerId = obj.CustomerId,
                                        BED = obj.BED,
                                        SellingProductId = obj.SellingProductId,
                                    });
                                }
                                else if (obj.BED > customerSellingProduct.BED)
                                {
                                    
                                        customerSellingProductObject.Add(new CustomerSellingProduct
                                        {
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,

                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProduct
                                        {
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                        });
                                }
                                
                            
                        }
                    }



                }
            #endregion

            List<CustomerSellingProduct> insertedObjects = new List<CustomerSellingProduct>();

            List<CustomerSellingProductClass> returnedData=new List<CustomerSellingProductClass>();
            bool insertActionSucc = dataManager.Insert(customerSellingProductObject, out  insertedObjects);
            foreach (CustomerSellingProduct customerSellingProduct in customerSellingProductObject)
            {
                if (customerSellingProduct.CustomerSellingProductId != 0)
                {
                    var obj = CustomerSellingProductDetailMapper(customerSellingProduct);
                    returnedData.Add(new CustomerSellingProductClass{
                        Status= Status.Deleted,
                        Entity = obj.Entity,
                        CustomerName = obj.CustomerName,
                        SellingProductName = obj.SellingProductName

                    });
                }
            }
            foreach (CustomerSellingProduct customerSellingProduct in insertedObjects)
            {
                var obj = CustomerSellingProductDetailMapper(customerSellingProduct);
                returnedData.Add(new CustomerSellingProductClass
                {
                    Status = Status.New,
                    Entity = obj.Entity,
                    CustomerName = obj.CustomerName,
                    SellingProductName = obj.SellingProductName
                });
            }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = returnedData;
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail> UpdateCustomerSellingProduct(CustomerSellingProduct customerSellingProduct)
        {
            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CustomerSellingProductDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            if (customerSellingProduct.BED < DateTime.Now)
            {
                return updateOperationOutput;
            }    
            bool updateActionSucc = dataManager.Update(customerSellingProduct);

            if (updateActionSucc)
            {
                CustomerSellingProductDetail customerSellingProductDetail = CustomerSellingProductDetailMapper(customerSellingProduct);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = customerSellingProductDetail;
            }

            return updateOperationOutput;
        }

        public IEnumerable<CustomerSellingProduct> GetCustomerSellingProductBySellingProduct(int sellingProductId)
        {
            var customerSellingProducts = GetCachedOrderedCustomerSellingProducts();
            return customerSellingProducts.Values.FindAllRecords(x => x.SellingProductId != sellingProductId && x.BED>DateTime.Now);
        }



        //public TOne.Entities.UpdateOperationOutput<object> DeleteCustomerSellingProduct(int customerSellingProductId)
        //{
        //    ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();

        //    TOne.Entities.UpdateOperationOutput<object> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<object>();
        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

        //    bool updateActionSucc = dataManager.Delete(customerSellingProductId);

        //    if (updateActionSucc)
        //    {
        //         List<CustomerSellingProductDetail> customerSellingProducts = GetCachedCustomerSellingProducts();
        //            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //         updateOperationOutput.UpdatedObject = customerSellingProducts.FindRecord(x => x.CustomerSellingProductId == customerSellingProductId);


        //    }

        //    return updateOperationOutput;
        //}
        #region Private Members

        Dictionary<int,CustomerSellingProduct> GetCachedOrderedCustomerSellingProducts()
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
                CarrierAccount carrierAccount=carrierAccountManager.GetCarrierAccount(customerSellingProduct.CustomerId);
            if(carrierAccount!=null)
                customerSellingProductDetail.CustomerName=carrierAccount.Name;
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
    public enum Status { New = 0, Updated = 1,Deleted=2 }
    public class CustomerSellingProductClass : CustomerSellingProductDetail
    {
        public Status Status { get; set; }
    }


}
