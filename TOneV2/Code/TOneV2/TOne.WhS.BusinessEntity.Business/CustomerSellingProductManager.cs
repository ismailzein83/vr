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
                 (input.Query.EffectiveDate == null || (prod.BED<input.Query.EffectiveDate && (prod.EED==null || prod.EED>input.Query.EffectiveDate)))
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
                            AllDestinations = customerSellingProduct.AllDestinations,
                            CustomerId = customerSellingProduct.CustomerId,
                            BED = customerSellingProduct.BED,
                            EED = customerSellingProduct.EED,
                            SellingProductId = customerSellingProduct.SellingProductId,
                            CustomerName = customerSellingProduct.CustomerName,
                            SellingProductName = customerSellingProduct.SellingProductName
                        });
                    }
                    else
                    {
                        foreach (CustomerSellingProductDetail obj in customerSellingProductList)
                        {
                            if (obj.SellingProductId == customerSellingProduct.SellingProductId)
                            {
                                if (obj.BED == customerSellingProduct.BED && (obj.EED < customerSellingProduct.EED || obj.EED > customerSellingProduct.EED || obj.EED == null))
                                {
                                    customerSellingProductObject.Add(new CustomerSellingProductDetail
                                    {
                                        AllDestinations = obj.AllDestinations,
                                        CustomerId = obj.CustomerId,
                                        BED = obj.BED,
                                        CustomerSellingProductId = obj.CustomerSellingProductId,
                                        EED = customerSellingProduct.EED,
                                        SellingProductId = obj.SellingProductId,
                                        CustomerName = obj.CustomerName,
                                        SellingProductName = obj.SellingProductName
                                    });
                                }
                                else if (obj.BED < customerSellingProduct.BED)
                                {
                                    if (obj.EED < customerSellingProduct.BED)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });

                                    }
                                    else if (obj.EED > customerSellingProduct.BED && (obj.EED < customerSellingProduct.EED || customerSellingProduct.EED == null))
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            EED = customerSellingProduct.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });

                                    }
                                    else if (obj.EED > customerSellingProduct.EED || obj.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = customerSellingProduct.BED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = (DateTime)customerSellingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });

                                    }
                                }
                                else if (obj.BED > customerSellingProduct.BED)
                                {
                                    if (obj.EED < customerSellingProduct.EED || customerSellingProduct.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                    }
                                    else if (obj.EED > customerSellingProduct.EED || obj.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            BED =(DateTime) customerSellingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                    }
                                }
                            }
                            else if (obj.SellingProductId != customerSellingProduct.SellingProductId)
                            {
                                if (obj.BED == customerSellingProduct.BED)
                                {
                                    if (obj.EED < customerSellingProduct.EED || customerSellingProduct.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            EED = customerSellingProduct.BED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                    }
                                    else if(obj.EED > customerSellingProduct.EED ||obj.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            EED = customerSellingProduct.BED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = (DateTime)customerSellingProduct.EED,
                                            EED = obj.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                    }
                                    
                                }
                                else if (obj.BED > customerSellingProduct.BED)
                                {
                                    if (obj.EED > customerSellingProduct.EED || obj.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            EED = obj.BED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED =(DateTime) customerSellingProduct.EED,
                                            EED = obj.EED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });

                                    }
                                    else if (obj.EED < customerSellingProduct.EED || customerSellingProduct.EED==null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            EED = obj.BED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                    }
                                }
                                 else if (obj.BED < customerSellingProduct.BED){
                                     
                                     if (obj.EED > customerSellingProduct.EED || obj.EED == null)
                                    {
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerSellingProductId = obj.CustomerSellingProductId,
                                            EED = customerSellingProduct.BED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = customerSellingProduct.AllDestinations,
                                            CustomerId = customerSellingProduct.CustomerId,
                                            BED = customerSellingProduct.BED,
                                            EED = customerSellingProduct.EED,
                                            SellingProductId = customerSellingProduct.SellingProductId,
                                            CustomerName = customerSellingProduct.CustomerName,
                                            SellingProductName = customerSellingProduct.SellingProductName
                                        });
                                        customerSellingProductObject.Add(new CustomerSellingProductDetail
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = (DateTime)customerSellingProduct.EED,
                                            EED = obj.EED,
                                            SellingProductId = obj.SellingProductId,
                                            CustomerName = obj.CustomerName,
                                            SellingProductName = obj.SellingProductName
                                        });
                                    }
                                     else if (obj.EED < customerSellingProduct.EED)
                                     {
                                         customerSellingProductObject.Add(new CustomerSellingProductDetail
                                         {
                                             AllDestinations = obj.AllDestinations,
                                             CustomerId = obj.CustomerId,
                                             BED = obj.BED,
                                             CustomerSellingProductId = obj.CustomerSellingProductId,
                                             EED = customerSellingProduct.BED,
                                             SellingProductId = obj.SellingProductId,
                                             CustomerName = obj.CustomerName,
                                             SellingProductName = obj.SellingProductName
                                         });
                                         customerSellingProductObject.Add(new CustomerSellingProductDetail
                                         {
                                             AllDestinations = customerSellingProduct.AllDestinations,
                                             CustomerId = customerSellingProduct.CustomerId,
                                             BED = customerSellingProduct.BED,
                                             EED = customerSellingProduct.EED,
                                             SellingProductId = customerSellingProduct.SellingProductId,
                                             CustomerName = customerSellingProduct.CustomerName,
                                             SellingProductName = customerSellingProduct.SellingProductName
                                         });
                                     }
                                     
                                   
                                }
                            }
                        }
                    }



                }
            #endregion

            List<CustomerSellingProductDetail> insertedObjects = new List<CustomerSellingProductDetail>();

            List<CustomerSellingProductClass> returnedData=new List<CustomerSellingProductClass>();
            bool insertActionSucc = dataManager.Insert(customerSellingProductObject, out  insertedObjects);
            //if (insertActionSucc)
            //{
            //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            // //   customerSellingProducts.CustomerSellingProductId = customerSellingProductId;
            //    insertOperationOutput.InsertedObject = dataManager.GetCustomerSellingProduct(customerSellingProductId);
            //}
            foreach (CustomerSellingProductDetail customerSellingProduct in customerSellingProductObject)
            {
                if (customerSellingProduct.CustomerSellingProductId != 0)
                {
                    returnedData.Add(new CustomerSellingProductClass{
                        AllDestinations=customerSellingProduct.AllDestinations,
                        Status= Status.Updated,
                        BED=customerSellingProduct.BED,
                        CustomerId=customerSellingProduct.CustomerId,
                        CustomerSellingProductId=customerSellingProduct.CustomerSellingProductId,
                        EED=customerSellingProduct.EED,
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
                    AllDestinations = customerSellingProduct.AllDestinations,
                    Status = Status.New,
                    BED = customerSellingProduct.BED,
                    CustomerId = customerSellingProduct.CustomerId,
                    CustomerSellingProductId = customerSellingProduct.CustomerSellingProductId,
                    EED = customerSellingProduct.EED,
                    SellingProductId = customerSellingProduct.SellingProductId,
                    CustomerName = customerSellingProduct.CustomerName,
                    SellingProductName=customerSellingProduct.SellingProductName
                });
            }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            // //   customerSellingProducts.CustomerSellingProductId = customerSellingProductId;
                insertOperationOutput.InsertedObject = returnedData;
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<object> DeleteCustomerSellingProduct(int customerSellingProductId)
        {
            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();

            TOne.Entities.UpdateOperationOutput<object> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

            bool updateActionSucc = dataManager.Delete(customerSellingProductId);

            if (updateActionSucc)
            {
                 List<CustomerSellingProductDetail> customerSellingProducts = GetCachedCustomerSellingProducts();
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                 updateOperationOutput.UpdatedObject = customerSellingProducts.FindRecord(x => x.CustomerSellingProductId == customerSellingProductId);


            }

            return updateOperationOutput;
        }
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
    public enum Status { New = 0, Updated = 1 }
    public class CustomerSellingProductClass : CustomerSellingProductDetail
    {
        public Status Status { get; set; }
    }


}
