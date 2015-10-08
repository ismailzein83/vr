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


        public TOne.Entities.InsertOperationOutput<CustomerPricingProductDetail> AddCustomerPricingProduct(CustomerPricingProduct customerPricingProduct)
        {

            TOne.Entities.InsertOperationOutput<CustomerPricingProductDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CustomerPricingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int customerPricingProductId = -1;
            ICustomerPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerPricingProductDataManager>();
            List<CustomerPricingProduct> customerPricingProductList = dataManager.GetCustomerPricingProductByCustomerID(customerPricingProduct.CustomerId);
            List<CustomerPricingProduct> customerPricingProductObject=new List<CustomerPricingProduct>();
            foreach (CustomerPricingProduct obj in customerPricingProductList)
            {
                if (obj.PricingProductId == customerPricingProduct.PricingProductId)
                {
                    if(obj.BED==customerPricingProduct.BED && (obj.EED<customerPricingProduct.EED || obj.EED>customerPricingProduct.EED))
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
                        else if (obj.EED > customerPricingProduct.BED && obj.EED < customerPricingProduct.EED)
                        {
                            customerPricingProductObject.Add(new CustomerPricingProduct
                            {
                                AllDestinations = obj.AllDestinations,
                                CustomerId = obj.CustomerId,
                                BED = obj.BED,
                                EED = customerPricingProduct.BED,
                                CustomerPricingProductId=obj.CustomerPricingProductId,
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
                                EED = customerPricingProduct.EED,
                                AllDestinations = obj.AllDestinations,
                                CustomerId = obj.CustomerId,
                                CustomerPricingProductId = obj.CustomerPricingProductId,
                                PricingProductId = obj.PricingProductId
                            });

                        }
                    }
                }
                else if (obj.PricingProductId != customerPricingProduct.PricingProductId)
                {
                    if (obj.BED == customerPricingProduct.BED && (obj.EED < customerPricingProduct.EED || obj.EED > customerPricingProduct.EED))
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
            bool insertActionSucc = dataManager.Insert(customerPricingProduct, out customerPricingProductId);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                customerPricingProduct.CustomerPricingProductId = customerPricingProductId;
                insertOperationOutput.InsertedObject = dataManager.GetCustomerPricingProduct(customerPricingProductId);
            }

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
}
