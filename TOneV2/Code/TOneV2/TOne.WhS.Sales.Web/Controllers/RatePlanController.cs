﻿using System;
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
        public IEnumerable<ZoneItem> GetZoneItems(GetZoneItemsInput input)
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
		[Route("GetCountryChanges")]
		public CountryChanges GetCountryChanges(int customerId)
		{
			var manager = new RatePlanDraftManager();
			return manager.GetCountryChanges(customerId);
		}

        [HttpGet]
        [Route("GetTQIMethods")]
        public IEnumerable<TQIMethodConfig> GetTQIMethods()
        {
            var manager = new RatePlanExtensionConfigManager();
            return manager.GetTQIMethods();
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

		[HttpGet]
		[Route("GetBulkActionTypeExtensionConfigs")]
		public IEnumerable<BulkActionTypeSettings> GetBulkActionTypeExtensionConfigs(SalePriceListOwnerType ownerType)
		{
			var manager = new RatePlanExtensionConfigManager();
			return manager.GetBulkActionTypeExtensionConfigs(ownerType);
		}

		[HttpGet]
		[Route("GetBulkActionZoneFilterTypeExtensionConfigs")]
		public IEnumerable<BulkActionZoneFilterTypeSettings> GetBulkActionZoneFilterTypeExtensionConfigs()
		{
			var manager = new RatePlanExtensionConfigManager();
			return manager.GetBulkActionZoneFilterTypeExtensionConfigs();
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

		[HttpPost]
		[Route("ApplyBulkActionToDraft")]
		public void ApplyBulkActionToDraft(ApplyActionToDraftInput input)
		{
			var manager = new RatePlanManager();
			manager.ApplyBulkActionToDraft(input);
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
        [Route("GetSaleAreaSettingsData")]
        public SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            var manager = new RatePlanManager();
            return manager.GetSaleAreaSettingsData();
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

        [HttpPost]
        [Route("GetCustomerDefaultInheritedService")]
        public SaleEntityService GetCustomerDefaultInheritedService(GetCustomerDefaultInheritedServiceInput input)
        {
            var manager = new DefaultItemManager();
            return manager.GetCustomerDefaultInheritedService(input);
        }

        [HttpPost]
        [Route("GetZoneInheritedService")]
        public SaleEntityService GetZoneInheritedService(GetZoneInheritedServiceInput input)
        {
            var manager = new RatePlanZoneManager();
            return manager.GetZoneInheritedService(input);
        }

		[HttpPost]
		[Route("GetFilteredSoldCountries")]
		public object GetFilteredSoldCountries(Vanrise.Entities.DataRetrievalInput<SoldCountryQuery> input)
		{
			var manager = new SoldCountryManager();
			return base.GetWebResponse(input, manager.GetFilteredSoldCountries(input));
		}
    }
}