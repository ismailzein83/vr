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
    public class CustomerPricingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerPricingProductDetail> GetFilteredCustomerPricingProducts(Vanrise.Entities.DataRetrievalInput<CustomerPricingProductQuery> input)
        {

            var allCustomerPricingProducts = GetCachedCustomerPricingProducts();

            Func<CustomerPricingProductDetail, bool> filterExpression = (prod) =>
                 (input.Query.EffectiveDate == null || (prod.BED<input.Query.EffectiveDate && (prod.EED==null || prod.EED>input.Query.EffectiveDate)))
                 &&
                 (input.Query.CustomersIds == null || input.Query.CustomersIds.Contains(prod.CustomerId))
                  &&
                 (input.Query.PricingProductsIds == null || input.Query.PricingProductsIds.Contains(prod.PricingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerPricingProducts.ToBigResult(input, filterExpression));
        }

        public CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId)
        {
            List<CustomerPricingProductDetail> customerPricingProducts = GetCachedCustomerPricingProducts();
            return customerPricingProducts.FindRecord(x => x.CustomerPricingProductId == customerPricingProductId);
        }


        public TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>> AddCustomerPricingProduct(List<CustomerPricingProductDetail> customerPricingProducts)
        {

            TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>> insertOperationOutput = new TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>>();

            //insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            //insertOperationOutput.InsertedObject = null;

          //  int customerPricingProductId = -1;
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            List<CustomerPricingProductDetail> customerPricingProductObject = new List<CustomerPricingProductDetail>();

            #region Insertion Rules
            foreach (CustomerPricingProductDetail customerPricingProduct in customerPricingProducts)
                {
                    List<CustomerPricingProductDetail> cashedCustomerPricingProducts = GetCachedCustomerPricingProducts();


                    IEnumerable<CustomerPricingProductDetail> customerPricingProductList = cashedCustomerPricingProducts.FindAllRecords(x => x.CustomerId == customerPricingProduct.CustomerId);
                    if (customerPricingProductList == null || customerPricingProductList.Count() == 0)
                    {
                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                        {
                            AllDestinations = customerPricingProduct.AllDestinations,
                            CustomerId = customerPricingProduct.CustomerId,
                            BED = customerPricingProduct.BED,
                            EED = customerPricingProduct.EED,
                            PricingProductId = customerPricingProduct.PricingProductId,
                            CustomerName = customerPricingProduct.CustomerName,
                            PricingProductName = customerPricingProduct.PricingProductName
                        });
                    }
                    else
                    {
                        foreach (CustomerPricingProductDetail obj in customerPricingProductList)
                        {
                            if (obj.PricingProductId == customerPricingProduct.PricingProductId)
                            {
                                if (obj.BED == customerPricingProduct.BED && (obj.EED < customerPricingProduct.EED || obj.EED > customerPricingProduct.EED || obj.EED == null))
                                {
                                    customerPricingProductObject.Add(new CustomerPricingProductDetail
                                    {
                                        AllDestinations = obj.AllDestinations,
                                        CustomerId = obj.CustomerId,
                                        BED = obj.BED,
                                        CustomerPricingProductId = obj.CustomerPricingProductId,
                                        EED = customerPricingProduct.EED,
                                        PricingProductId = obj.PricingProductId,
                                        CustomerName = obj.CustomerName,
                                        PricingProductName = obj.PricingProductName
                                    });
                                }
                                else if (obj.BED < customerPricingProduct.BED)
                                {
                                    if (obj.EED < customerPricingProduct.BED)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });

                                    }
                                    else if (obj.EED > customerPricingProduct.BED && (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED == null))
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            EED = customerPricingProduct.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });

                                    }
                                    else if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = customerPricingProduct.BED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });

                                    }
                                }
                                else if (obj.BED > customerPricingProduct.BED)
                                {
                                    if (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                    }
                                    else if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            BED =(DateTime) customerPricingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                    }
                                }
                            }
                            else if (obj.PricingProductId != customerPricingProduct.PricingProductId)
                            {
                                if (obj.BED == customerPricingProduct.BED)
                                {
                                    if (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                    }
                                    else if(obj.EED > customerPricingProduct.EED ||obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                    }
                                    
                                }
                                else if (obj.BED > customerPricingProduct.BED)
                                {
                                    if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = obj.BED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED =(DateTime) customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });

                                    }
                                    else if (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED==null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = obj.BED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                    }
                                }
                                 else if (obj.BED < customerPricingProduct.BED){
                                     
                                     if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId,
                                            CustomerName = customerPricingProduct.CustomerName,
                                            PricingProductName = customerPricingProduct.PricingProductName
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = obj.PricingProductId,
                                            CustomerName = obj.CustomerName,
                                            PricingProductName = obj.PricingProductName
                                        });
                                    }
                                     else if (obj.EED < customerPricingProduct.EED)
                                     {
                                         customerPricingProductObject.Add(new CustomerPricingProductDetail
                                         {
                                             AllDestinations = obj.AllDestinations,
                                             CustomerId = obj.CustomerId,
                                             BED = obj.BED,
                                             CustomerPricingProductId = obj.CustomerPricingProductId,
                                             EED = customerPricingProduct.BED,
                                             PricingProductId = obj.PricingProductId,
                                             CustomerName = obj.CustomerName,
                                             PricingProductName = obj.PricingProductName
                                         });
                                         customerPricingProductObject.Add(new CustomerPricingProductDetail
                                         {
                                             AllDestinations = customerPricingProduct.AllDestinations,
                                             CustomerId = customerPricingProduct.CustomerId,
                                             BED = customerPricingProduct.BED,
                                             EED = customerPricingProduct.EED,
                                             PricingProductId = customerPricingProduct.PricingProductId,
                                             CustomerName = customerPricingProduct.CustomerName,
                                             PricingProductName = customerPricingProduct.PricingProductName
                                         });
                                     }
                                     
                                   
                                }
                            }
                        }
                    }



                }
            #endregion

            List<CustomerPricingProductDetail> insertedObjects = new List<CustomerPricingProductDetail>();

            List<CustomerPricingProductClass> returnedData=new List<CustomerPricingProductClass>();
            bool insertActionSucc = dataManager.Insert(customerPricingProductObject, out  insertedObjects);
            //if (insertActionSucc)
            //{
            //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            // //   customerPricingProducts.CustomerPricingProductId = customerPricingProductId;
            //    insertOperationOutput.InsertedObject = dataManager.GetCustomerPricingProduct(customerPricingProductId);
            //}
            foreach (CustomerPricingProductDetail customerPricingProduct in customerPricingProductObject)
            {
                if (customerPricingProduct.CustomerPricingProductId != 0)
                {
                    returnedData.Add(new CustomerPricingProductClass{
                        AllDestinations=customerPricingProduct.AllDestinations,
                        Status= Status.Updated,
                        BED=customerPricingProduct.BED,
                        CustomerId=customerPricingProduct.CustomerId,
                        CustomerPricingProductId=customerPricingProduct.CustomerPricingProductId,
                        EED=customerPricingProduct.EED,
                        PricingProductId = customerPricingProduct.PricingProductId,
                        CustomerName = customerPricingProduct.CustomerName,
                        PricingProductName = customerPricingProduct.PricingProductName

                    });
                }
            }
            foreach (CustomerPricingProductDetail customerPricingProduct in insertedObjects)
            {
                returnedData.Add(new CustomerPricingProductClass
                {
                    AllDestinations = customerPricingProduct.AllDestinations,
                    Status = Status.New,
                    BED = customerPricingProduct.BED,
                    CustomerId = customerPricingProduct.CustomerId,
                    CustomerPricingProductId = customerPricingProduct.CustomerPricingProductId,
                    EED = customerPricingProduct.EED,
                    PricingProductId = customerPricingProduct.PricingProductId,
                    CustomerName = customerPricingProduct.CustomerName,
                    PricingProductName=customerPricingProduct.PricingProductName
                });
            }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            // //   customerPricingProducts.CustomerPricingProductId = customerPricingProductId;
                insertOperationOutput.InsertedObject = returnedData;
            return insertOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteCustomerPricingProduct(int customerPricingProductId)
        {
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(customerPricingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
        #region Private Members

        List<CustomerPricingProductDetail> GetCachedCustomerPricingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCustomerPricingProducts",
               () =>
               {
                   ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
                   return dataManager.GetCustomerPricingProducts();
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerPricingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCustomerPricingProductsUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
    public enum Status { New = 0, Updated = 1 }
    public class CustomerPricingProductClass : CustomerPricingProductDetail
    {
        public Status Status { get; set; }
    }


}
