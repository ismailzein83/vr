﻿using System;
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
