using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RevenueComparison")]
    public class RevenueComparisonController:BaseAPIController
    {
        RevenueComparisonManager _manager = new RevenueComparisonManager();

        [HttpPost]
        [Route("GetFilteredRevenueComparisons")]
        public object GetFilteredRevenueComparisons(Vanrise.Entities.DataRetrievalInput<RevenueComparisonQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRevenueComparisons(input));
        }

    }
}