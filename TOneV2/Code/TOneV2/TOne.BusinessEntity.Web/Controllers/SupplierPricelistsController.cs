using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Web.Controllers
{
    public class SupplierPricelistsController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            SupplierPricelistsManager manager = new SupplierPricelistsManager();
            return GetWebResponse(input, manager.GetSupplierPriceLists(input));
        }
        [HttpPost]
        public object GetSupplierPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            SupplierPricelistsManager manager = new SupplierPricelistsManager();
            return GetWebResponse(input, manager.GetSupplierPriceListDetails(input));
        }
    }
}