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

            Func<CustomerSellingProductDetail, bool> filterExpression = (prod) =>
                 (input.Query.EffectiveDate == null || (prod.BED<input.Query.EffectiveDate))
                 &&
                 (input.Query.CustomersIds == null || input.Query.CustomersIds.Contains(prod.CustomerId))
                  &&
                 (input.Query.SellingProductsIds == null || input.Query.SellingProductsIds.Contains(prod.SellingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSellingProducts.ToBigResult(input, filterExpression));
        }

        public CustomerSellingProductDetail GetCustomerSellingProduct(int customerSellingProductId)
        {
            List<CustomerSellingProductDetail> customerSellingProducts = GetCachedCustomerSellingProducts();
            return customerSellingProducts.FindRecord(x => x.CustomerSellingProductId == customerSellingProductId);
        }

        public CustomerSellingProduct GetEffectiveSellingProduct(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }


        public TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>> AddCustomerSellingProduct(List<CustomerSellingProductDetail> customerSellingProducts)
        {

            TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>> insertOperationOutput = new TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>>();

            //insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            //insertOperationOutput.InsertedObject = null;

          //  int customerSellingProductId = -1;
            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            List<CustomerSellingProductDetail> customerSellingProductObject = new List<CustomerSellingProductDetail>();

            #region Insertion Rules
            foreach (CustomerSellingProductDetail customerSellingProduct in customerSellingProducts)
                {
                    List<CustomerSellingProductDetail> cashedCustomerSellingProducts = GetCachedCustomerSellingProducts();


                    IEnumerable<CustomerSellingProductDetail> customerSellingProductList = cashedCustomerSellingProducts.FindAllRecords(x => x.CustomerId == customerSellingProduct.CustomerId);
                    if (customerSellingProductList == null || customerSellingProductList.Count() == 0)
                    {
                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                        {
                            CustomerId = customerSellingProduct.CustomerId,
                            BED = customerSellingProduct.BED,
                            SellingProductId = customerSellingProduct.SellingProductId,
                            CustomerName = customerSellingProduct.CustomerName,
                            SellingProductName = customerSellingProduct.SellingProductName
                        });
                    }
                    else
                    {
                        foreach (CustomerSellingProductDetail obj in customerSellingProductList)
                        {
                                if (obj.BED < customerSellingProduct.BED)
                                {
                                    customerSellingProductObject.Add(new CustomerSellingProductDetail
                                    {
                                        CustomerId = obj.CustomerId,
                                        BED = obj.BED,
                                        SellingProductId = obj.SellingProductId,
                                        CustomerName = obj.CustomerName,
                                        SellingProductName = obj.SellingProductName
                                    });
                                }
                                else if (obj.BED > customerSellingProduct.BED)
                                {
                                    
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                }
                                
                            
                        }
                    }



                }
            #endregion

            List<CustomerSellingProductDetail> insertedObjects = new List<CustomerSellingProductDetail>();

            List<CustomerSellingProductClass> returnedData=new List<CustomerSellingProductClass>();
            bool insertActionSucc = dataManager.Insert(customerSellingProductObject, out  insertedObjects);
            foreach (CustomerSellingProductDetail customerSellingProduct in customerSellingProductObject)
            {
                if (customerSellingProduct.CustomerSellingProductId != 0)
                {
                    returnedData.Add(new CustomerSellingProductClass{
                        Status= Status.Deleted,
                        BED=customerSellingProduct.BED,
                        CustomerId=customerSellingProduct.CustomerId,
                        CustomerSellingProductId=customerSellingProduct.CustomerSellingProductId,
                        SellingProductId = customerSellingProduct.SellingProductId,
                        CustomerName = customerSellingProduct.CustomerName,
                        SellingProductName = customerSellingProduct.SellingProductName

                    });
                }
            }
            foreach (CustomerSellingProductDetail customerSellingProduct in insertedObjects)
            {
                returnedData.Add(new CustomerSellingProductClass
                {
                    Status = Status.New,
                    BED = customerSellingProduct.BED,
                    CustomerId = customerSellingProduct.CustomerId,
                    CustomerSellingProductId = customerSellingProduct.CustomerSellingProductId,
                    SellingProductId = customerSellingProduct.SellingProductId,
                    CustomerName = customerSellingProduct.CustomerName,
                    SellingProductName=customerSellingProduct.SellingProductName
                });
            }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = returnedData;
            return insertOperationOutput;
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

        List<CustomerSellingProductDetail> GetCachedCustomerSellingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerSellingProducts",
               () =>
               {
                   ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
                   return dataManager.GetCustomerSellingProducts();
               });
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
