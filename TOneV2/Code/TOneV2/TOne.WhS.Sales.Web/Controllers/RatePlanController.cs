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
        public IEnumerable<RatePlanItem> GetRatePlanItems(RatePlanItemInput query)
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

        [HttpGet]
        [Route("GetRatePlan")]
        public RatePlan GetRatePlan(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            RatePlanManager manager = new RatePlanManager();
            return manager.GetRatePlan(ownerType, ownerId, status);
        }

        [HttpPost]
        [Route("SaveRatePlanDraft")]
        public void SaveRatePlanDraft(RatePlan draft)
        {
            RatePlanManager manager = new RatePlanManager();
            manager.SaveRatePlanDraft(draft);
        }
    }
}