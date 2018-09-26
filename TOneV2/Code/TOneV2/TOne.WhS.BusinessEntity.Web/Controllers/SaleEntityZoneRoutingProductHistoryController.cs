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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleEntityZoneRoutingProductHistory")]
    public class SaleEntityZoneRoutingProductHistoryController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleEntityZoneRoutingProductHistoryRecords")]
        public object GetFilteredSaleEntityZoneRoutingProductHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
        {
            return GetWebResponse(input, new SaleEntityZoneRoutingProductHistoryManager().GetFilteredSaleEntityZoneRoutingProductHistoryRecords(input), "Zone Routing Product");
        }
    }
}