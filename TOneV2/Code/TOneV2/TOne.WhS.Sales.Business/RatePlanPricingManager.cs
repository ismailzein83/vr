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
        #region Apply Calculated Rates

        public IEnumerable<InvalidRate> ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            IEnumerable<InvalidRate> invalidRates;
            var ratePlanManager = new RatePlanManager();

            int? sellingNumberPlanId = ratePlanManager.GetSellingNumberPlanId(input.OwnerType, input.OwnerId);
            int? sellingProductId = ratePlanManager.GetSellingProductId(input.OwnerType, input.OwnerId, input.EffectiveOn, false);

            ApplyCalculatedRates(input, sellingNumberPlanId.Value, sellingProductId.Value, out invalidRates);

            return invalidRates;
        }

        private IEnumerable<InvalidRate> ApplyCalculatedRates(ApplyCalculatedRatesInput input, int sellingNumberPlanId, int sellingProductId, out IEnumerable<InvalidRate> invalidRates)
        {
            invalidRates = null;

            IEnumerable<ZoneItem> zoneItems = GetZoneItemsWithCalculatedRate(input, sellingNumberPlanId, sellingProductId);

            if (zoneItems != null)
            {
                Changes newDraft = GetNewDraft(zoneItems, input.EffectiveOn, input.CurrencyId, out invalidRates);

                if (newDraft != null)
                {
                    var draftManager = new RatePlanDraftManager();
                    draftManager.SaveDraft(input.OwnerType, input.OwnerId, newDraft);
                }
            }

            return invalidRates;
        }

        private IEnumerable<ZoneItem> GetZoneItemsWithCalculatedRate(ApplyCalculatedRatesInput input, int sellingNumberPlanId, int sellingProductId)
        {
            // Get the sale zones of the owner
            RatePlanZoneManager ratePlanZoneManager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = ratePlanZoneManager.GetFilteredZones(input.OwnerType, input.OwnerId, sellingNumberPlanId, input.EffectiveOn, input.CountryIds, null, input.ZoneNameFilterType, input.ZoneNameFilter);

            if (zones == null)
                return null;

            // Create a list of zone items, and set the effective routing product for each
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            int? customerId = null;
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                customerId = input.OwnerId;

            var draftManager = new RatePlanDraftManager();
            Changes changes = draftManager.GetDraft(input.OwnerType, input.OwnerId);

            var zoneRateManager = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, input.EffectiveOn, changes, input.CurrencyId);
            var routingProductSetter = new ZoneRoutingProductManager(sellingProductId, customerId, input.EffectiveOn, changes);

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
            return zoneItems.FindAllRecords(itm => itm.CalculatedRate != null);
        }

        private Changes GetNewDraft(IEnumerable<ZoneItem> zoneItems, DateTime effectiveOn, int currencyId, out IEnumerable<InvalidRate> invalidRates)
        {
            // Create a list of zone changes, each having a new rate, from the calculated rates
            List<ZoneChanges> zoneChanges = new List<ZoneChanges>();

            var ratePlanManager = new RatePlanManager();
            RatePlanSettingsData ratePlanSettings = ratePlanManager.GetRatePlanSettingsData();

            var invalidRateList = new List<InvalidRate>();
            DateTime newRateBED;

            foreach (ZoneItem zoneItem in zoneItems)
            {
                if (!zoneItem.CalculatedRate.HasValue)
                    continue;

                if (zoneItem.CalculatedRate.Value > 0)
                {
                    newRateBED = (!zoneItem.CurrentRate.HasValue || zoneItem.CalculatedRate.Value > zoneItem.CurrentRate.Value) ?
                    effectiveOn.AddDays(ratePlanSettings.IncreasedRateDayOffset) :
                    effectiveOn.AddDays(ratePlanSettings.DecreasedRateDayOffset);

                    DraftRateToChange newRate = new DraftRateToChange()
                    {
                        ZoneId = zoneItem.ZoneId,
                        NormalRate = zoneItem.CalculatedRate.Value,
                        BED = newRateBED
                    };

                    var zoneDraft = new ZoneChanges() { ZoneId = zoneItem.ZoneId, ZoneName = zoneItem.ZoneName, NewRates = new List<DraftRateToChange>() { newRate } };
                    zoneChanges.Add(zoneDraft);
                }
                else
                {
                    invalidRateList.Add(new InvalidRate()
                    {
                        ZoneId = zoneItem.ZoneId,
                        ZoneName = zoneItem.ZoneName,
                        Rate = zoneItem.CalculatedRate.Value
                    });
                }
            }

            invalidRates = invalidRateList.Count > 0 ? invalidRateList : null;

            return new Changes()
            {
                CurrencyId = currencyId,
                ZoneChanges = zoneChanges
            };
        }

        #endregion
    }
}
