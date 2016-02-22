using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.SupplierPricelist.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Customer")]
    public class CP_SupPriceList_CustomerController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomers")]
        public CustomerOutput GetCustomers(ref byte[] maxTimeStamp, int nbOfRows)
        {
            CustomerManager manager = new CustomerManager();
            return manager.GetCustomers(ref maxTimeStamp, nbOfRows);
        }

        //[HttpGet]
        //[Route("GetCustomerTemplates")]
        //public List<TemplateConfig> GetCustomerTemplates()
        //{
        //    CustomerManager manager = new CustomerManager();
        //    return manager.GetConnectorTemplates();
        //}
        //[HttpPost]
        //[Route("AddCustomer")]
        //public new InsertOperationOutput<Customer> AddCustomer(Customer input)
        //{
        //    CustomerManager manager = new CustomerManager();
        //    return manager.AddCustomer(input);
        //}

        //[HttpPost]
        //[Route("UpdateCustomer")]
        //public new UpdateOperationOutput<Customer> UpdateCustomer(Customer input)
        //{
        //    CustomerManager manager = new CustomerManager();
        //    return manager.UpdateCustomer(input);
        //}

    }
}