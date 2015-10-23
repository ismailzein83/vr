using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class CustomerSellingProductController:BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCustomerSellingProducts(Vanrise.Entities.DataRetrievalInput<CustomerSellingProductQuery> input)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return GetWebResponse(input, manager.GetFilteredCustomerSellingProducts(input));
        }
        [HttpGet]
        public CustomerSellingProductDetail GetCustomerSellingProduct(int customerSellingProductId)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.GetCustomerSellingProduct(customerSellingProductId);
        }
        [HttpPost]
        public TOne.Entities.InsertOperationOutput<List<CustomerSellingProductClass>> AddCustomerSellingProduct(List<CustomerSellingProductDetail> customerSellingProducts)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.AddCustomerSellingProduct(customerSellingProducts);
        }
        [HttpGet]
        public TOne.Entities.UpdateOperationOutput<object> DeleteCustomerSellingProduct(int customerSellingProductId)
        {
            CustomerSellingProductManager manager = new CustomerSellingProductManager();
            return manager.DeleteCustomerSellingProduct(customerSellingProductId);
        } 
    }
}