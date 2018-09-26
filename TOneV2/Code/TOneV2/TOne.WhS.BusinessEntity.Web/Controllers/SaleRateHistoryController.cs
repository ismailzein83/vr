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
        [Route("GetFilteredSaleRateHistoryRecords")]
        public object GetFilteredSaleRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
        {
            return GetWebResponse(input, new SaleRateHistoryManager().GetFilteredSaleRateHistoryRecords(input), "Sale Rate History");
        }
    }
}