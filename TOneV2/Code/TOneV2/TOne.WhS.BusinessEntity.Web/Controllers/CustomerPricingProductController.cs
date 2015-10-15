﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class CustomerPricingProductController:BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCustomerPricingProducts(Vanrise.Entities.DataRetrievalInput<CustomerPricingProductQuery> input)
        {
            CustomerPricingProductManager manager = new CustomerPricingProductManager();
            return GetWebResponse(input, manager.GetFilteredCustomerPricingProducts(input));
        }
        [HttpGet]
        public CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId)
        {
            CustomerPricingProductManager manager = new CustomerPricingProductManager();
            return manager.GetCustomerPricingProduct(customerPricingProductId);
        }
        [HttpPost]
        public TOne.Entities.InsertOperationOutput<List<CustomerPricingProductClass>> AddCustomerPricingProduct(List<CustomerPricingProductDetail> customerPricingProducts)
        {
            CustomerPricingProductManager manager = new CustomerPricingProductManager();
            return manager.AddCustomerPricingProduct(customerPricingProducts);
        }
        [HttpGet]
        public TOne.Entities.UpdateOperationOutput<object> DeleteCustomerPricingProduct(int customerPricingProductId)
        {
            CustomerPricingProductManager manager = new CustomerPricingProductManager();
            return manager.DeleteCustomerPricingProduct(customerPricingProductId);
        } 
    }
}