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

        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            var ratePlanManager = new RatePlanManager();

            int? sellingNumberPlanId = ratePlanManager.GetSellingNumberPlanId(input.OwnerType, input.OwnerId);
            int? sellingProductId = ratePlanManager.GetSellingProductId(input.OwnerType, input.OwnerId, input.EffectiveOn, false);

            ApplyCalculatedRates(input.OwnerType, input.OwnerId, (int)sellingNumberPlanId, (int)sellingProductId, input.EffectiveOn, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, input.CostCalculationMethods, input.SelectedCostCalculationMethodConfigId, input.RateCalculationMethod, input.CurrencyId);
        }

        public void ApplyCalculatedRates(SalePriceListOwnerType ownerType, int ownerId, int sellingNumberPlanId, int sellingProductId, DateTime effectiveOn, int routingDatabaseId, int policyConfigId, int numberOfOptions, List<CostCalculationMethod> costCalculationMethods, int selectedCostCalculationMethodConfigId, RateCalculationMethod rateCalculationMethod, int currencyId)
        {
            IEnumerable<ZoneItem> zoneItems = GetZoneItemsWithCalculatedRate(ownerType, ownerId, sellingNumberPlanId, sellingProductId, effectiveOn, routingDatabaseId, policyConfigId, numberOfOptions, costCalculationMethods, selectedCostCalculationMethodConfigId, rateCalculationMethod, currencyId);

            if (zoneItems == null)
                return;

            Changes newChanges = GetZoneChanges(zoneItems, effectiveOn);

            if (newChanges == null)
                return;

            var draftManager = new RatePlanDraftManager();
            draftManager.SaveDraft(ownerType, ownerId, newChanges);
        }

        private IEnumerable<ZoneItem> GetZoneItemsWithCalculatedRate(SalePriceListOwnerType ownerType, int ownerId, int sellingNumberPlanId, int sellingProductId, DateTime effectiveOn, int routingDatabaseId, int policyConfigId, int numberOfOptions, List<CostCalculationMethod> costCalculationMethods, int selectedCostCalculationMethodConfigId, RateCalculationMethod rateCalculationMethod, int currencyId)
        {
            // Get the sale zones of the owner
            RatePlanZoneManager ratePlanZoneManager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = ratePlanZoneManager.GetZones(ownerType, ownerId, sellingNumberPlanId, effectiveOn);

            if (zones == null)
                return null;

            // Create a list of zone items, and set the effective routing product for each
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            int? customerId = null;
            if (ownerType == SalePriceListOwnerType.Customer)
                customerId = ownerId;

            var draftManager = new RatePlanDraftManager();
            Changes changes = draftManager.GetDraft(ownerType, ownerId);
            ZoneRoutingProductManager routingProductSetter = new ZoneRoutingProductManager(sellingProductId, customerId, effectiveOn, changes);

            foreach (SaleZone zone in zones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    ZoneId = zone.SaleZoneId,
                    ZoneName = zone.Name
                };

                routingProductSetter.SetZoneRoutingProduct(zoneItem);
                zoneItems.Add(zoneItem);
            }

            // Set the route options, calculate the costs, and calculate the rate for all zone items
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
            ZoneRouteOptionManager routeOptionSetter = new ZoneRouteOptionManager(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, costCalculationMethods, selectedCostCalculationMethodConfigId, rateCalculationMethod, currencyId);

            routeOptionSetter.SetZoneRouteOptionProperties(zoneItems);
            return zoneItems.FindAllRecords(itm => itm.CalculatedRate != null);
        }

        private Changes GetZoneChanges(IEnumerable<ZoneItem> zoneItems, DateTime effectiveOn)
        {
            // Create a list of zone changes, each having a new rate, from the calculated rates
            List<ZoneChanges> zoneChanges = new List<ZoneChanges>();

            foreach (ZoneItem zoneItem in zoneItems)
            {
                DraftRateToChange newRate = new DraftRateToChange()
                {
                    ZoneId = zoneItem.ZoneId,
                    NormalRate = (decimal)zoneItem.CalculatedRate,
                    BED = effectiveOn
                };

                ZoneChanges zoneItemChanges = new ZoneChanges() { ZoneId = zoneItem.ZoneId, ZoneName = zoneItem.ZoneName, NewRates = new List<DraftRateToChange>() { newRate } };
                zoneChanges.Add(zoneItemChanges);
            }

            return new Changes() { ZoneChanges = zoneChanges };
        }

        #endregion
    }
}
