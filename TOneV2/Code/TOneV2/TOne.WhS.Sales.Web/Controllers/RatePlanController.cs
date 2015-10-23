using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities.Queries;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlan")]
    public class RatePlanController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRatePlanItems")]
        public object GetFilteredRatePlanItems(DataRetrievalInput<RatePlanQuery> input)
        {
            RatePlanManager manager = new RatePlanManager();
            return GetWebResponse(input, manager.GetFilteredRatePlanItems(input));
        }
    }
}