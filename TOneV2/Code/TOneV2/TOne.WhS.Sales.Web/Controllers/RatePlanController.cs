using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.Queries;
using TOne.WhS.Sales.Entities.RatePlanning;
using TOne.WhS.Sales.Entities.RatePlanning.Input;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlan")]
    public class RatePlanController : BaseAPIController
    {
        [HttpGet]
        [Route("GetZoneLetters")]
        public IEnumerable<char> GetZoneLetters(RatePlanOwnerType ownerType, int ownerId)
        {
            RatePlanManager manager = new RatePlanManager();
            return manager.GetZoneLetters(ownerType, ownerId);
        }

        [HttpPost]
        [Route("GetZoneItems")]
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemInput input)
        {
            RatePlanManager manager = new RatePlanManager();
            return manager.GetZoneItems(input);
        }

        [HttpPost]
        [Route("SavePriceList")]
        public void SavePriceList(SalePriceListInput input)
        {
            RatePlanManager manager = new RatePlanManager();
            manager.SavePriceList(input);
        }

        //[HttpPost]
        //[Route("SaveRatePlanDraft")]
        //public void SaveRatePlanDraft(RatePlan draft)
        //{
        //    RatePlanManager manager = new RatePlanManager();
        //    manager.SaveRatePlanDraft(draft);
        //}
    }
}