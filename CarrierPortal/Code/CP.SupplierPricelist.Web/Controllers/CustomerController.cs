using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.SupplierPricelist.Web.Controllers
{
    
    [RoutePrefix(Constants.ROUTE_PREFIX + "Customer")]
    [JSONWithTypeAttribute]
    public class CP_SupPriceList_CustomerController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomer")]
        public Customer GetCustomer(int customerId)
        {
            CustomerManager manager = new CustomerManager();
            return manager.GetCustomer(customerId);
        }

        [HttpGet]
        [Route("GetCustomerTemplates")]
        public List<TemplateConfig> GetCustomerTemplates()
        {
            CustomerManager manager = new CustomerManager();
            return manager.GetConnectorTemplates();
        }
        [HttpPost]
        [Route("AddCustomer")]
        public InsertOperationOutput<CustomerDetail> AddCustomer(Customer input)
        {
            CustomerManager manager = new CustomerManager();
            return manager.AddCustomer(input);
        }
        [HttpPost]
        [Route("UpdateCustomer")]
        public new UpdateOperationOutput<CustomerDetail> UpdateRule(Customer input)
        {
            CustomerManager manager = new CustomerManager();
            return manager.UpdateCustomer(input);
        }


        [HttpPost]
        [Route("GetFilteredCustomers")]
        public object GetFilteredSellingRules(DataRetrievalInput<Customer> input)
        {
            CustomerManager manager = new CustomerManager();
            return GetWebResponse(input, manager.GetFilteredCustomers(input));
        }
    }
}