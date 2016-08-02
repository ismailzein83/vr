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
        [Route("ValidateCustomer")]
        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            return _manager.ValidateCustomer(customerId, effectiveOn);
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
        public IEnumerable<CostCalculationMethodSetting> GetCostCalculationMethodTemplates()
        {
            return _manager.GetCostCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetRateCalculationMethodTemplates")]
        public IEnumerable<RateCalculationMethodSetting> GetRateCalculationMethodTemplates()
        {
            return _manager.GetRateCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetChangesSummary")]
        public ChangesSummary GetChangesSummary(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.GetChangesSummary(ownerType, ownerId);
        }

        [HttpPost]
        [Route("GetFilteredZoneRateChanges")]
        public object GetFilteredZoneRateChanges(Vanrise.Entities.DataRetrievalInput<ZoneRateChangesQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredZoneRateChanges(input));
        }
        
        [HttpPost]
        [Route("GetFilteredZoneRoutingProductChanges")]
        public object GetFilteredZoneRoutingProductChanges(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductChangesQuery> input)
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
        public void SaveChanges(SaveChangesInput input)
        {
            _manager.SaveChanges(input);
        }

        [HttpPost]
        [Route("ApplyCalculatedRates")]
        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            _manager.ApplyCalculatedRates(input);
        }

        [HttpGet]
        [Route("CheckIfDraftExists")]
        public bool CheckIfDraftExists(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.CheckIfDraftExists(ownerType, ownerId);
        }

        [HttpGet]
        [Route("DeleteDraft")]
        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            return _manager.DeleteDraft(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetRatePlanSettingsData")]
        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            return _manager.GetRatePlanSettingsData();
        }
    }
}