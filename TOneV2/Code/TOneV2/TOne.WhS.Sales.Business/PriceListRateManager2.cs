using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager2
    {
        public void ProcessCountryRates(IProcessRatesContext context)
        {
            ProcessRates(context.RatesToChange, context.RatesToClose, context.ExistingRates, context.ExistingZones);
            context.NewRates = context.RatesToChange.SelectMany(itm => itm.NewRates);
            context.ChangedRates = context.ExistingRates.Where(itm => itm.ChangedRate != null).Select(itm => itm.ChangedRate);
        }

        private ExistingZonesByName StructureExistingZonesByName(IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZonesList = null;

            foreach (ExistingZone item in existingZones)
            {
                if (!existingZonesByName.TryGetValue(item.Name, out existingZonesList))
                {
                    existingZonesList = new List<ExistingZone>();
                    existingZonesByName.Add(item.Name, existingZonesList);
                }

                existingZonesList.Add(item);
            }

            return existingZonesByName;

        }

        private ExistingRatesByZoneName StructureExistingRatesByZoneName(IEnumerable<ExistingRate> existingRates)
        {
            ExistingRatesByZoneName existingRatesByZoneName = new ExistingRatesByZoneName();
            List<ExistingRate> existingRatesList = null;

            foreach (ExistingRate item in existingRates)
            {
                if (!existingRatesByZoneName.TryGetValue(item.ParentZone.Name, out existingRatesList))
                {
                    existingRatesList = new List<ExistingRate>();
                    existingRatesByZoneName.Add(item.ParentZone.Name, existingRatesList);
                }

                existingRatesList.Add(item);
            }

            return existingRatesByZoneName;  
        }

        private void ProcessRates(IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose, IEnumerable<ExistingRate> existingRates, IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingRatesByZoneName existingRatesByZoneName = StructureExistingRatesByZoneName(existingRates);
            
            foreach (RateToChange rateToChange in ratesToChange)
            {
                List<NewRate> ratesToAdd = new List<NewRate>();
                List<ExistingRate> matchExistingRates;
                if (existingRatesByZoneName.TryGetValue(rateToChange.ZoneName, out matchExistingRates))
                {
                    bool shouldNotAddRate;
                    Decimal? recentRateValue;
                    CloseExistingOverlapedRates(rateToChange, matchExistingRates, out shouldNotAddRate, out recentRateValue);
                    if (!shouldNotAddRate)
                    {
                        if (recentRateValue.HasValue)
                        {
                            rateToChange.ChangeType = rateToChange.NormalRate > recentRateValue.Value ? RateChangeType.Increase : RateChangeType.Decrease;
                            rateToChange.RecentNormalRate = recentRateValue;
                        }
                        else
                        {
                            rateToChange.ChangeType = RateChangeType.New;
                        }
                        ProcessRateToChange(rateToChange, existingZonesByName);
                    }
                }
                else
                {
                    rateToChange.ChangeType = RateChangeType.New;
                    ProcessRateToChange(rateToChange, existingZonesByName);
                }
            }

            foreach (RateToClose rateToClose in ratesToClose)
            {
                List<ExistingRate> matchExistingRates;
                if (existingRatesByZoneName.TryGetValue(rateToClose.ZoneName, out matchExistingRates))
                    CloseExistingRates(rateToClose, matchExistingRates);
            }
        }

        private void CloseExistingOverlapedRates(RateToChange rateToChange, List<ExistingRate> matchExistingRates, out bool shouldNotAddRate, out Decimal? recentRateValue)
        {
            shouldNotAddRate = false;
            recentRateValue = null;
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.RateEntity.BED < rateToChange.BED)
                    recentRateValue = existingRate.RateEntity.NormalRate;
                if (existingRate.IsOverlapedWith(rateToChange))
                {
                    DateTime existingRateEED = Utilities.Max(rateToChange.BED, existingRate.BED);
                    existingRate.ChangedRate = new ChangedRate
                    {
                        RateId = existingRate.RateEntity.SaleRateId,
                        EED = existingRateEED
                    };
                    rateToChange.ChangedExistingRates.Add(existingRate);
                }
            }

        }

        private void ProcessRateToChange(RateToChange rateToChange, ExistingZonesByName existingZones)
        {
            List<ExistingZone> matchExistingZones;
            existingZones.TryGetValue(rateToChange.ZoneName, out matchExistingZones);
            

            DateTime currentRateBED = rateToChange.BED;
            bool shouldAddMoreRates = true;
            foreach (var zone in matchExistingZones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentRateBED) && rateToChange.EED.VRGreaterThan(zone.BED))
                {
                    AddNewRate(rateToChange, ref currentRateBED, zone, out shouldAddMoreRates);
                    if (!shouldAddMoreRates)
                        break;
                }
            }
        }

        private void AddNewRate(RateToChange rateToChange, ref DateTime currentRateBED, ExistingZone zone, out bool shouldAddMoreRates)
        {
            shouldAddMoreRates = false;
            var newRate = new NewRate
            {
                NormalRate = rateToChange.NormalRate,
                OtherRates = rateToChange.OtherRates,
                CurrencyId = rateToChange.CurrencyId,
                Zone = zone,
                BED = zone.BED > currentRateBED ? zone.BED : currentRateBED,
                EED = rateToChange.EED
            };
            if (newRate.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
            {
                newRate.EED = zone.EED;
                currentRateBED = newRate.EED.Value;
                shouldAddMoreRates = true;
            }

            zone.NewRates.Add(newRate);

            rateToChange.NewRates.Add(newRate);
        }

        private bool SameRates(RateToChange importedRate, ExistingRate existingRate)
        {
            return importedRate.BED == existingRate.RateEntity.BED
               && importedRate.NormalRate == existingRate.RateEntity.NormalRate
               && importedRate.CurrencyId == existingRate.RateEntity.CurrencyId
               //TODO: compare CurrencyId of the Pricelists
               && SameRateOtherRates(importedRate, existingRate);
        }

        private bool SameRateOtherRates(RateToChange importedRate, ExistingRate existingRate)
        {
            int importedRatesCount = importedRate.OtherRates != null ? importedRate.OtherRates.Count : 0;
            int existingRatesCount = existingRate.RateEntity.OtherRates != null ? existingRate.RateEntity.OtherRates.Count : 0;
            if (importedRatesCount != existingRatesCount)
                return false;
            if (importedRatesCount == 0)
                return true;
            //if rates Count is > 0, then both dictionaries are not null

            foreach(var importedRateEntry in importedRate.OtherRates)
            {
                Decimal matchExistingRate;
                if (!existingRate.RateEntity.OtherRates.TryGetValue(importedRateEntry.Key, out matchExistingRate) || importedRateEntry.Value != matchExistingRate)
                    return false;
            }
            return true;
        }

        private void CloseExistingRates(RateToClose rateToClose, List<ExistingRate> matchExistingRates)
        {
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.EED.VRGreaterThan(rateToClose.CloseEffectiveDate))
                {
                    existingRate.ChangedRate = new ChangedRate
                    {
                        RateId = existingRate.RateEntity.SaleRateId,
                        EED = Utilities.Max(rateToClose.CloseEffectiveDate, existingRate.BED)
                    };
                    rateToClose.ChangedExistingRates.Add(existingRate);
                }
            }
        }
    }
}
