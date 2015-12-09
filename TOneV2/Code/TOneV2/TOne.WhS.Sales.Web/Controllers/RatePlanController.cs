using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
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
            return _manager.GetZoneItems(input);
        }

        [HttpGet]
        [Route("GetZoneItem")]
        public ZoneItem GetZoneItem(int routingDatabaseId, int policyConfigId, int numberOfOptions, SalePriceListOwnerType ownerType, int ownerId, long zoneId, List<CostCalculationMethod> costCalculationMethods)
        {
            return _manager.GetZoneItem(ownerType, ownerId, routingDatabaseId, policyConfigId, numberOfOptions, zoneId, costCalculationMethods);
        }

        [HttpGet]
        [Route("GetDefaultItem")]
        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetDefaultItem(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetCostCalculationMethodTemplates")]
        public List<TemplateConfig> GetCostCalculationMethodTemplates()
        {
            return _manager.GetCostCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetRecentChanges")]
        public IEnumerable<Changes> GetRecentChanges(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetRecentChanges(ownerType, ownerId);
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