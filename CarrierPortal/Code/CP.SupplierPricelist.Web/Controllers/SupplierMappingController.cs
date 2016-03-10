using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;

namespace CP.SupplierPricelist.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierMapping")]
    public class CP_SupPriceList_SupplierMappingController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomerSuppliers")]
        public IEnumerable<SupplierInfo> GetCustomerSuppliers(string serializedFilter)
        {
            SupplierInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SupplierInfoFilter>(serializedFilter) : null;
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.GetCustomerSuppliers(filter);
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
        public CustomerSupplierMapping GetCustomerSupplierMapping(int UserId)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.GetCustomerSupplierMapping(UserId);
        }

        [HttpPost]
        [Route("AddCustomerSupplierMapping")]
        public object AddCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.AddCustomerSupplierMapping(customerSupplierMapping);
        }
        [HttpPost]
        [Route("UpdateCustomerSupplierMapping")]
        public object UpdateCustomerSupplierMapping(CustomerSupplierMapping customerSupplierMapping)
        {
            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.UpdateCustomerSupplierMapping(customerSupplierMapping);
        }
        [HttpGet]
        [Route("DeleteCustomerSupplierMapping")]
        public DeleteOperationOutput<CustomerSupplierMappingDetail> DeleteCustomerSupplierMapping(int supplierMappingId)
        {

            CustomerSupplierMappingManager manager = new CustomerSupplierMappingManager();
            return manager.DeleteCustomerSupplierMapping(supplierMappingId);
        }
    }
}