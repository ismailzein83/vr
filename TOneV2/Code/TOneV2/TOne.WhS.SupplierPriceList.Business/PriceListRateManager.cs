using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(List<ImportedRate> importedRates, ExistingRatesByZoneName existingRates, List<ChangedRate> changedRates, ZonesByName newAndExistingZones, ExistingZonesByName existingZones)
        {
            CloseRatesForClosedZones(existingZones.SelectMany(itm => itm.Value), changedRates);
            foreach(var importedRate in importedRates)
            {
                List<NewRate> ratesToAdd = new List<NewRate>();
                List<ExistingRate> matchExistingRates;
                if(existingRates.TryGetValue(importedRate.ZoneName, out matchExistingRates))
                {
                    bool shouldNotAddRate;
                    Decimal? recentRateValue;
                    CloseExistingOverlapedRates(importedRate, matchExistingRates, out shouldNotAddRate, out recentRateValue);
                    if (!shouldNotAddRate)
                    {
                        if (recentRateValue.HasValue)
                            importedRate.ChangeType = importedRate.NormalRate > recentRateValue.Value ? RateChangeType.Increase : RateChangeType.Decrease;
                        else
                            importedRate.ChangeType = RateChangeType.New;
                        AddImportedRate(importedRate, newAndExistingZones, existingZones);
                    }
                }
                else
                {
                    importedRate.ChangeType = RateChangeType.New;
                    AddImportedRate(importedRate, newAndExistingZones, existingZones);
                }
            }
        }

        private void CloseRatesForClosedZones(IEnumerable<ExistingZone> existingZones, List<ChangedRate> changedRates)
        {
            foreach (var existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null)
                {
                    DateTime zoneEED = existingZone.ChangedZone.EED;
                    if (existingZone.ExistingRates != null)
                    {
                        foreach (var existingRate in existingZone.ExistingRates)
                        {
                            DateTime? rateEED = existingRate.EED;
                            if (rateEED.VRGreaterThan(zoneEED))
                            {
                                if (existingRate.ChangedRate == null)
                                {
                                    existingRate.ChangedRate = new ChangedRate
                                    {
                                        RateId = existingRate.RateEntity.SupplierRateId
                                    };
                                    changedRates.Add(existingRate.ChangedRate);
                                }
                                DateTime rateBED = existingRate.RateEntity.BeginEffectiveDate;
                                existingRate.ChangedRate.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }

        private void CloseExistingOverlapedRates(ImportedRate importedRate, List<ExistingRate> matchExistingRates, out bool shouldNotAddRate, out Decimal? recentRateValue)
        {
            shouldNotAddRate = false;
            recentRateValue = null;
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.RateEntity.BeginEffectiveDate < importedRate.BED)
                    recentRateValue = existingRate.RateEntity.NormalRate;
                if (existingRate.EED.VRGreaterThan(importedRate.BED) && importedRate.EED.VRGreaterThan(existingRate.RateEntity.BeginEffectiveDate))
                {
                    if (SameRates(importedRate, existingRate))
                    {
                        if (importedRate.EED == existingRate.EED)
                        {
                            shouldNotAddRate = true;
                            break;
                        }
                        else if(importedRate.EED.HasValue && importedRate.EED.VRLessThan(existingRate.EED))
                        {
                            existingRate.ChangedRate = new ChangedRate
                            {
                                RateId = existingRate.RateEntity.SupplierRateId,
                                EED = importedRate.EED.Value 
                            };
                            importedRate.ChangedExistingRates.Add(existingRate);
                        }

                    }
                    else
                    {
                        DateTime existingRateEED = importedRate.BED > existingRate.RateEntity.BeginEffectiveDate ? importedRate.BED : existingRate.RateEntity.BeginEffectiveDate;
                        existingRate.ChangedRate = new ChangedRate
                        {
                            RateId = existingRate.RateEntity.SupplierRateId,
                            EED = existingRateEED
                        };
                        importedRate.ChangedExistingRates.Add(existingRate);
                    }
                }
            }

        }

        private void AddImportedRate(ImportedRate importedRate, ZonesByName newAndExistingZones, ExistingZonesByName existingZones)
        {
            List<IZone> zones;
            if (!newAndExistingZones.TryGetValue(importedRate.ZoneName, out zones))
            {
                zones = new List<IZone>();
                List<ExistingZone> matchExistingZones;
                if (existingZones.TryGetValue(importedRate.ZoneName, out matchExistingZones))
                    zones.AddRange(matchExistingZones);
                newAndExistingZones.Add(importedRate.ZoneName, zones);
            }
            DateTime currentRateBED = importedRate.BED;
            bool shouldAddMoreRates = true;
            foreach (var zone in zones.OrderBy(itm => itm.BED))
            {
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentRateBED))
                {
                    AddNewRate(importedRate, ref currentRateBED, zone, out shouldAddMoreRates);
                    if (!shouldAddMoreRates)
                        break;
                }
            }
        }

        private void AddNewRate(ImportedRate importedRate, ref DateTime currentRateBED, IZone zone, out bool shouldAddMoreRates)
        {
            shouldAddMoreRates = false;
            var newRate = new NewRate
            {
                NormalRate = importedRate.NormalRate,
                OtherRates = importedRate.OtherRates,
                CurrencyId = importedRate.CurrencyId,
                Zone = zone,
                BED = zone.BED > currentRateBED ? zone.BED : currentRateBED,
                EED = importedRate.EED
            };
            if (newRate.EED.VRGreaterThan(zone.EED))//this means that zone has EED value
            {
                newRate.EED = zone.EED;
                currentRateBED = newRate.EED.Value;
                shouldAddMoreRates = true;
            }

            zone.NewRates.Add(newRate);

            importedRate.NewRates.Add(newRate);
        }

        private bool SameRates(ImportedRate importedRate, ExistingRate existingRate)
        {
            return importedRate.BED == existingRate.RateEntity.BeginEffectiveDate
               && importedRate.NormalRate == existingRate.RateEntity.NormalRate
               && importedRate.CurrencyId == existingRate.RateEntity.CurrencyId
               && SameRateOtherRates(importedRate, existingRate);
        }

        private bool SameRateOtherRates(ImportedRate importedRate, ExistingRate existingRate)
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
    }
}
