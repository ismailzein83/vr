using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;
using TOne.WhS.Sales.Entities.RatePlanning.Input;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlan")]
    public class RatePlanController : BaseAPIController
    {
        private RatePlanManager _manager;

        public RatePlanController()
        {
            _manager = new RatePlanManager();
        }

        [HttpGet]
        [Route("GetZoneLetters")]
        public IEnumerable<char> GetZoneLetters(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetZoneLetters(ownerType, ownerId);
        }

        [HttpPost]
        [Route("GetZoneItems")]
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemInput input)
        {
            RatePlanManager _manager = new RatePlanManager();
            return _manager.GetZoneItems(input);
        }

        [HttpGet]
        [Route("GetDefaultItem")]
        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetDefaultItem(ownerType, ownerId);
        }

        [HttpGet]
        [Route("SavePriceList")]
        public void SavePriceList(SalePriceListOwnerType ownerType, int ownerId)
        {
            _manager.SavePriceList(ownerType, ownerId);
        }

        [HttpPost]
        [Route("SaveChanges")]
        public bool SaveChanges(SaveChangesInput input)
        {
            return _manager.SaveChanges(input);
        }
    }
}