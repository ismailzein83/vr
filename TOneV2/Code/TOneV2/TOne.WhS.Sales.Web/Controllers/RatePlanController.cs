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
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            return _manager.GetZoneItems(input);
        }

        [HttpPost]
        [Route("GetZoneItem")]
        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            return _manager.GetZoneItem(input);
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
        [Route("GetChangesSummary")]
        public ChangesSummary GetChangesSummary(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetChangesSummary(ownerType, ownerId);
        }

        [HttpPost]
        [Route("GetFilteredZoneRateChanges")]
        public object GetFilteredZoneRateChanges(Vanrise.Entities.DataRetrievalInput<ZoneRateChangesInput> input)
        {
            return GetWebResponse(input, _manager.GetFilteredZoneRateChanges(input));
        }
        
        [HttpPost]
        [Route("GetFilteredZoneRoutingProductChanges")]
        public object GetFilteredZoneRoutingProductChanges(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductChangesInput> input)
        {
            return GetWebResponse(input, _manager.GetFilteredZoneRoutingProductChanges(input));
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