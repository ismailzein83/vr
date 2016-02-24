using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.SupplierPricelist.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierMapping")]
    public class CP_SupPriceList_SupplierMappingController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomerSuppliers")]
        public IEnumerable<SupplierInfo> GetCustomerSuppliers()
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.GetCustomerSuppliers();
        }


        [HttpPost]
        [Route("GetFilteredCustomerSupplierMappings")]
        public object GetFilteredCustomerSupplierMappings(Vanrise.Entities.DataRetrievalInput<CustomerSupplierMappingQuery> input)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return GetWebResponse(input, manager.GetFilteredCustomerSupplierMappings(input));
        }
        [HttpGet]
        [Route("GetCustomerSupplierMapping")]
        public CustomerSupplierMapping GetCustomerSupplierMapping(int supplierMappingId)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.GetCustomerSupplierMapping(supplierMappingId);
        }

        [HttpPost]
        [Route("AddCustomerSupplierMapping")]
        public object AddCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.AddCustomerSupplierMapping(customerSupplierMapping);
        }
    }
}