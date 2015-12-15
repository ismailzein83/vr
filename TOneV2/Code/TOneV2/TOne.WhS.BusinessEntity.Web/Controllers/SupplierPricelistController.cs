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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPricelist")]
    public class SupplierPricelistController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSupplierPricelist")]
        public IEnumerable<SupplierPriceList> GetFilteredSupplierPricelist(SupplierPricelistFilter input)
        {
            SupplierPriceListManager
                manager = new SupplierPriceListManager();
            return manager.GetFilteredSupplierPriceLists(input);
        }
    }
}