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
    }
}