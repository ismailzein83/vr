using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanPricingManager
    {
        public CalculatedRates TryApplyCalculatedRates(TryApplyCalculatedRatesInput input)
        {
            CalculatedRates calculatedRates = GetCalculatedRates(input);

            if (calculatedRates.InvalidCalculatedRates.Count > 0)
                return calculatedRates;

            else if (calculatedRates.ValidCalculatedRates.Count > 0)
            {
                var context = new ApplyCalculatedRatesInput()
                {
                    OwnerType = input.OwnerType,
                    OwnerId = input.OwnerId,
                    CalculatedRates = calculatedRates.ValidCalculatedRates,
                    EffectiveOn = input.EffectiveOn,
                    CurrencyId = input.CurrencyId
                };
                ApplyCalculatedRates(context);
            }

            return null;
        }
        private CalculatedRates GetCalculatedRates(TryApplyCalculatedRatesInput input)
        {
            CalculatedRates rates = new CalculatedRates()
            {
                ValidCalculatedRates = new List<CalculatedZoneRate>(),
                InvalidCalculatedRates = new List<CalculatedZoneRate>()
            };

            IEnumerable<ZoneItem> zoneItems = GetZoneItemsWithCalculatedRate(input);

            foreach (ZoneItem zoneItem in zoneItems)
            {
                var rate = new CalculatedZoneRate()
                {
                    ZoneId = zoneItem.ZoneId,
                    ZoneName = zoneItem.ZoneName,
                    CurrentRate = zoneItem.CurrentRate,
                    CalculatedRate = zoneItem.CalculatedRate.Value
                };
                if (zoneItem.CalculatedRate.Value > 0)
                    rates.ValidCalculatedRates.Add(rate);
                else
                    rates.InvalidCalculatedRates.Add(rate);
            }

            return rates;
        }
        private IEnumerable<ZoneItem> GetZoneItemsWithCalculatedRate(TryApplyCalculatedRatesInput input)
        {
            var ratePlanManager = new RatePlanManager();

            int? sellingNumberPlanId = ratePlanManager.GetSellingNumberPlanId(input.OwnerType, input.OwnerId);
            int? sellingProductId = ratePlanManager.GetSellingProductId(input.OwnerType, input.OwnerId, input.EffectiveOn, false);

            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");
            if (!sellingProductId.HasValue)
                throw new NullReferenceException("sellingProductId");

            // Get the sale zones of the owner
            RatePlanZoneManager ratePlanZoneManager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = ratePlanZoneManager.GetFilteredZones(input.OwnerType, input.OwnerId, sellingNumberPlanId.Value, input.EffectiveOn, input.CountryIds, null, input.ZoneNameFilterType, input.ZoneNameFilter);

            if (zones == null)
                return null;

            // Create a list of zone items, and set the effective routing product for each
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            int? customerId = null;
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                customerId = input.OwnerId;

            var draftManager = new RatePlanDraftManager();
            Changes changes = draftManager.GetDraft(input.OwnerType, input.OwnerId);

            var zoneRateManager = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId.Value, input.EffectiveOn, changes, input.CurrencyId);
            var routingProductSetter = new ZoneRoutingProductManager(sellingProductId.Value, customerId, input.EffectiveOn, changes);

            foreach (SaleZone zone in zones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    ZoneId = zone.SaleZoneId,
                    ZoneName = zone.Name
                };

                zoneRateManager.SetZoneRate(zoneItem);
                routingProductSetter.SetZoneRoutingProduct(zoneItem);

                zoneItems.Add(zoneItem);
            }

            // Set the route options, calculate the costs, and calculate the rate for all zone items
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
            ZoneRouteOptionManager routeOptionSetter = new ZoneRouteOptionManager(input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, rpZones, input.CostCalculationMethods, input.SelectedCostCalculationMethodConfigId, input.RateCalculationMethod, input.CurrencyId);

            routeOptionSetter.SetZoneRouteOptionProperties(zoneItems);
            return zoneItems.FindAllRecords(x => x.CalculatedRate != null);
        }

        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            Changes newDraft = GetNewDraft(input.CalculatedRates, input.EffectiveOn, input.CurrencyId);

            if (newDraft != null)
            {
                var draftManager = new RatePlanDraftManager();
                draftManager.SaveDraft(input.OwnerType, input.OwnerId, newDraft);
            }
        }
        private Changes GetNewDraft(IEnumerable<CalculatedZoneRate> calculatedRates, DateTime effectiveOn, int currencyId)
        {
            Changes newDraft = null;

            if (calculatedRates == null)
                return newDraft;

            newDraft = new Changes()
            {
                CurrencyId = currencyId,
                ZoneChanges = new List<ZoneChanges>()
            };

            var ratePlanManager = new RatePlanManager();
            RatePlanSettingsData ratePlanSettings = ratePlanManager.GetRatePlanSettingsData();

            DateTime newRateBED;

            foreach (CalculatedZoneRate calculatedRate in calculatedRates)
            {
                newRateBED = (!calculatedRate.CurrentRate.HasValue || calculatedRate.CalculatedRate > calculatedRate.CurrentRate.Value) ?
                    effectiveOn.AddDays(ratePlanSettings.IncreasedRateDayOffset) :
                    effectiveOn.AddDays(ratePlanSettings.DecreasedRateDayOffset);

                DraftRateToChange newRate = new DraftRateToChange()
                {
                    ZoneId = calculatedRate.ZoneId,
                    NormalRate = calculatedRate.CalculatedRate,
                    BED = newRateBED
                };

                var zoneDraft = new ZoneChanges()
                {
                    ZoneId = calculatedRate.ZoneId,
                    ZoneName = calculatedRate.ZoneName,
                    NewRates = new List<DraftRateToChange>() { newRate }
                };

                newDraft.ZoneChanges.Add(zoneDraft);
            }

            return newDraft;
        }
    }
}
