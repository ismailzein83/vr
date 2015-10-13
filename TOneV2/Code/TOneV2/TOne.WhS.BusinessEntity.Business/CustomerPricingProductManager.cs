using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerPricingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerPricingProductDetail> GetFilteredCustomerPricingProducts(Vanrise.Entities.DataRetrievalInput<CustomerPricingProductQuery> input)
        {
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCustomerPricingProducts(input));
        }

        public CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId)
        {
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            return dataManager.GetCustomerPricingProduct(customerPricingProductId);
        }


        public TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>> AddCustomerPricingProduct(List<CustomerPricingProduct> customerPricingProducts)
        {

            TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>> insertOperationOutput = new TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>>();

            //insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            //insertOperationOutput.InsertedObject = null;

          //  int customerPricingProductId = -1;
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            List<CustomerPricingProduct> customerPricingProductObject = new List<CustomerPricingProduct>();

            #region Insertion Rules
            foreach (CustomerPricingProduct customerPricingProduct in customerPricingProducts)
                {
                    List<CustomerPricingProduct> customerPricingProductList = dataManager.GetCustomerPricingProductByCustomerID(customerPricingProduct.CustomerId);
                    if (customerPricingProductList==null || customerPricingProductList.Count == 0)
                    {
                        customerPricingProductObject.Add(new CustomerPricingProduct
                        {
                            AllDestinations = customerPricingProduct.AllDestinations,
                            CustomerId = customerPricingProduct.CustomerId,
                            BED = customerPricingProduct.BED,
                            EED = customerPricingProduct.EED,
                            PricingProductId = customerPricingProduct.PricingProductId
                        });
                    }
                    else
                    {
                        foreach (CustomerPricingProduct obj in customerPricingProductList)
                        {
                            if (obj.PricingProductId == customerPricingProduct.PricingProductId)
                            {
                                if (obj.BED == customerPricingProduct.BED && (obj.EED < customerPricingProduct.EED || obj.EED > customerPricingProduct.EED || obj.EED == null))
                                {
                                    customerPricingProductObject.Add(new CustomerPricingProduct
                                    {
                                        AllDestinations = obj.AllDestinations,
                                        CustomerId = obj.CustomerId,
                                        BED = obj.BED,
                                        CustomerPricingProductId = obj.CustomerPricingProductId,
                                        EED = customerPricingProduct.EED,
                                        PricingProductId = obj.PricingProductId
                                    });
                                }
                                else if (obj.BED < customerPricingProduct.BED)
                                {
                                    if (obj.EED < customerPricingProduct.BED)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });

                                    }
                                    else if (obj.EED > customerPricingProduct.BED && (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED == null))
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            EED = customerPricingProduct.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });

                                    }
                                    else if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = obj.BED,
                                            EED = customerPricingProduct.BED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            PricingProductId = obj.PricingProductId
                                        });

                                    }
                                }
                                else if (obj.BED > customerPricingProduct.BED)
                                {
                                    if (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                    }
                                    else if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = obj.BED,
                                            EED = obj.BED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            BED =(DateTime) customerPricingProduct.EED,
                                            EED = obj.EED,
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            PricingProductId = customerPricingProduct.PricingProductId
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
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                    }
                                    else if(obj.EED > customerPricingProduct.EED ||obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                    }
                                    
                                }
                                else if (obj.BED > customerPricingProduct.BED)
                                {
                                    if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = obj.BED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED =(DateTime) customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = obj.PricingProductId
                                        });

                                    }
                                    else if (obj.EED < customerPricingProduct.EED || customerPricingProduct.EED==null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = obj.BED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                    }
                                }
                                 else if (obj.BED < customerPricingProduct.BED){
                                     
                                     if (obj.EED > customerPricingProduct.EED || obj.EED == null)
                                    {
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = obj.BED,
                                            CustomerPricingProductId = obj.CustomerPricingProductId,
                                            EED = customerPricingProduct.BED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = customerPricingProduct.AllDestinations,
                                            CustomerId = customerPricingProduct.CustomerId,
                                            BED = customerPricingProduct.BED,
                                            EED = customerPricingProduct.EED,
                                            PricingProductId = customerPricingProduct.PricingProductId
                                        });
                                        customerPricingProductObject.Add(new CustomerPricingProduct
                                        {
                                            AllDestinations = obj.AllDestinations,
                                            CustomerId = obj.CustomerId,
                                            BED = (DateTime)customerPricingProduct.EED,
                                            EED = obj.EED,
                                            PricingProductId = obj.PricingProductId
                                        });
                                    }
                                     else if (obj.EED < customerPricingProduct.EED)
                                     {
                                         customerPricingProductObject.Add(new CustomerPricingProduct
                                         {
                                             AllDestinations = obj.AllDestinations,
                                             CustomerId = obj.CustomerId,
                                             BED = obj.BED,
                                             CustomerPricingProductId = obj.CustomerPricingProductId,
                                             EED = customerPricingProduct.BED,
                                             PricingProductId = obj.PricingProductId
                                         });
                                         customerPricingProductObject.Add(new CustomerPricingProduct
                                         {
                                             AllDestinations = customerPricingProduct.AllDestinations,
                                             CustomerId = customerPricingProduct.CustomerId,
                                             BED = customerPricingProduct.BED,
                                             EED = customerPricingProduct.EED,
                                             PricingProductId = customerPricingProduct.PricingProductId
                                         });
                                     }
                                     
                                   
                                }
                            }
                        }
                    }



                }
            #endregion

            List<CustomerPricingProduct> insertedObjects = new List<CustomerPricingProduct>();

            List<CustomerPricingProductClass> returnedData=new List<CustomerPricingProductClass>();
            bool insertActionSucc = dataManager.Insert(customerPricingProductObject, out  insertedObjects);
            //if (insertActionSucc)
            //{
            //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            // //   customerPricingProducts.CustomerPricingProductId = customerPricingProductId;
            //    insertOperationOutput.InsertedObject = dataManager.GetCustomerPricingProduct(customerPricingProductId);
            //}
            foreach (CustomerPricingProduct customerPricingProduct in customerPricingProductObject)
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

                    });
                }
            }
            foreach (CustomerPricingProduct customerPricingProduct in insertedObjects)
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
    }
    public enum Status { New = 0, Updated = 1 }
    public class CustomerPricingProductClass : CustomerPricingProduct
    {
        public Status Status { get; set; }
    }
}
