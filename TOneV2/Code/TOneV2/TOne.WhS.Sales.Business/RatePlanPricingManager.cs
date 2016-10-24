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
            var rates = new CalculatedRates()
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
					ZoneBED = zoneItem.ZoneBED,
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
            Changes draft = draftManager.GetDraft(input.OwnerType, input.OwnerId);
            IEnumerable<ZoneChanges> zoneDrafts = draft != null ? draft.ZoneChanges : null;

            var zoneRateManager = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId.Value, input.EffectiveOn, draft, input.CurrencyId);
            var rpManager = new ZoneRPManager(input.OwnerType, input.OwnerId, input.EffectiveOn, draft);
            
            foreach (SaleZone zone in zones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    ZoneId = zone.SaleZoneId,
                    ZoneName = zone.Name,
					ZoneBED = zone.BED
                };

                ZoneChanges zoneDraft = zoneDrafts != null ? zoneDrafts.FindRecord(x => x.ZoneId == zone.SaleZoneId) : null;

                zoneRateManager.SetZoneRate(zoneItem);

                if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                    rpManager.SetSellingProductZoneRP(zoneItem, input.OwnerId, zoneDraft);
                else
                    rpManager.SetCustomerZoneRP(zoneItem, input.OwnerId, sellingProductId.Value, zoneDraft);

                zoneItems.Add(zoneItem);
            }

            // Set the route options, calculate the costs, and calculate the rate for all zone items
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
            ZoneRouteOptionManager routeOptionSetter = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, rpZones, input.CostCalculationMethods, input.SelectedCostCalculationMethodConfigId, input.RateCalculationMethod, input.CurrencyId);

            routeOptionSetter.SetZoneRouteOptionProperties(zoneItems);
            return zoneItems.FindAllRecords(x => x.CalculatedRate != null);
        }

        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            Changes newDraft = GetNewDraft(input.OwnerType, input.OwnerId, input.CalculatedRates, input.EffectiveOn, input.CurrencyId);

            if (newDraft != null)
            {
                var draftManager = new RatePlanDraftManager();
                draftManager.SaveDraft(input.OwnerType, input.OwnerId, newDraft);
            }
        }
        private Changes GetNewDraft(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<CalculatedZoneRate> calculatedRates, DateTime effectiveOn, int currencyId)
        {
            if (calculatedRates == null)
                return null;

            Changes existingDraft = new RatePlanDraftManager().GetDraft(ownerType, ownerId);
            IEnumerable<ZoneChanges> existingZoneDrafts = (existingDraft != null) ? existingDraft.ZoneChanges : new List<ZoneChanges>();

            var newDraft = new Changes()
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
                    Rate = calculatedRate.CalculatedRate,
                };

				newRate.BED = new List<DateTime>() { newRateBED, calculatedRate.ZoneBED }.Max();

                ZoneChanges zoneDraft = existingZoneDrafts.FindRecord(x => x.ZoneId == calculatedRate.ZoneId);
                var newRates = new List<DraftRateToChange>();

                if (zoneDraft != null)
                {
                    if (zoneDraft.NewRates != null)
                    {
                        IEnumerable<DraftRateToChange> newOtherRates = zoneDraft.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
                        newRates = new List<DraftRateToChange>(newOtherRates);
                    }
                }
                else
                {
                    zoneDraft = new ZoneChanges()
                    {
                        ZoneId = calculatedRate.ZoneId,
                        ZoneName = calculatedRate.ZoneName
                    };
                }

                newRates.Add(newRate);
                zoneDraft.NewRates = newRates;

                newDraft.ZoneChanges.Add(zoneDraft);
            }

            return newDraft;
        }
    }
}
