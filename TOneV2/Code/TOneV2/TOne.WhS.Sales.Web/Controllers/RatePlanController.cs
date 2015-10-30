using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.Queries;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlan")]
    public class RatePlanController : BaseAPIController
    {
        [HttpPost]
        [Route("GetRatePlanItems")]
        public List<RatePlanItem> GetRatePlanItems(RatePlanItemInput query)
        {
            RatePlanManager manager = new RatePlanManager();
            return manager.GetRatePlanItems(query);
        }

        [HttpPost]
        [Route("SavePriceList")]
        public void SavePriceList(SalePriceListInput input)
        {
            RatePlanManager manager = new RatePlanManager();
            manager.SavePriceList(input);
        }
    }
}