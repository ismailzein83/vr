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
        [HttpGet]
        [Route("ValidateCustomer")]
        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            var manager = new RatePlanManager();
            return manager.ValidateCustomer(customerId, effectiveOn);
        }

        [HttpPost]
        [Route("GetZoneLetters")]
        public IEnumerable<char> GetZoneLetters(ZoneLettersInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneLetters(input);
        }

        [HttpPost]
        [Route("GetZoneItems")]
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneItems(input);
        }

        [HttpPost]
        [Route("GetZoneItem")]
        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            var manager = new RatePlanManager();
            return manager.GetZoneItem(input);
        }

        [HttpGet]
        [Route("GetDefaultItem")]
        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var manager = new DefaultItemManager();
            return manager.GetDefaultItem(ownerType, ownerId, effectiveOn);
        }

        [HttpGet]
        [Route("GetCostCalculationMethodTemplates")]
        public IEnumerable<CostCalculationMethodSetting> GetCostCalculationMethodTemplates()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetCostCalculationMethodTemplates();
        }

        [HttpGet]
        [Route("GetRateCalculationMethodTemplates")]
        public IEnumerable<RateCalculationMethodSetting> GetRateCalculationMethodTemplates()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetRateCalculationMethodTemplates();
        }

        [HttpPost]
        [Route("SaveChanges")]
        public void SaveChanges(SaveChangesInput input)
        {
            var manager = new RatePlanDraftManager();
            manager.SaveDraft(input.OwnerType, input.OwnerId, input.NewChanges);
        }

        [HttpPost]
        [Route("TryApplyCalculatedRates")]
        public CalculatedRates TryApplyCalculatedRates(TryApplyCalculatedRatesInput input)
        {
            var manager = new RatePlanPricingManager();
            return manager.TryApplyCalculatedRates(input);
        }

        [HttpPost]
        [Route("ApplyCalculatedRates")]
        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            var manager = new RatePlanPricingManager();
            manager.ApplyCalculatedRates(input);
        }

        [HttpGet]
        [Route("CheckIfDraftExists")]
        public bool CheckIfDraftExists(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.DoesDraftExist(ownerType, ownerId);
        }

        [HttpGet]
        [Route("DeleteDraft")]
        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.DeleteDraft(ownerType, ownerId);
        }

        [HttpGet]
        [Route("GetRatePlanSettingsData")]
        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            var manager = new RatePlanManager();
            return manager.GetRatePlanSettingsData();
        }

        [HttpGet]
        [Route("GetDraftCurrencyId")]
        public int? GetDraftCurrencyId(SalePriceListOwnerType ownerType, int ownerId)
        {
            var manager = new RatePlanDraftManager();
            return manager.GetDraftCurrencyId(ownerType, ownerId);
        }

        [HttpGet]
        [Route("DeleteChangedRates")]
        public void DeleteChangedRates(SalePriceListOwnerType ownerType, int ownerId, int newCurrencyId)
        {
            var manager = new RatePlanDraftManager();
            manager.DeleteChangedRates(ownerType, ownerId, newCurrencyId);
        }

        [HttpGet]
        [Route("GetInheritedService")]
        public SaleEntityService GetInheritedService(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, long? zoneId = null)
        {
            var manager = new DefaultItemManager();
            return manager.GetInheritedService(ownerType, ownerId, effectiveOn, zoneId);
        }
    }
}