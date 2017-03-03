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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleRateHistory")]
    public class SaleRateHistoryController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSellingProductZoneRateHistoryRecords")]
        public object GetFilteredSellingProductZoneRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SellingProductZoneRateHistoryQuery> input)
        {
            return GetWebResponse(input, new SaleRateHistoryManager().GetFilteredSellingProductZoneRateHistoryRecords(input));
        }

        [HttpPost]
        [Route("GetFilteredCustomerZoneRateHistoryRecords")]
        public object GetFilteredCustomerZoneRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<CustomerZoneRateHistoryQuery> input)
        {
            return GetWebResponse(input, new SaleRateHistoryManager().GetFilteredCustomerZoneRateHistoryRecords(input));
        }
    }
}