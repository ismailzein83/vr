using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(IProcessCountryRatesContext context)
        {
            ProcessCountryRates(context.ImportedRates, context.ExistingRates, context.NewAndExistingZones, context.ExistingZones);
            context.NewRates = context.ImportedRates.SelectMany(itm => itm.NewRates);
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

        private void ProcessCountryRates(IEnumerable<ImportedRate> importedRates, IEnumerable<ExistingRate> existingRates, ZonesByName newAndExistingZones, IEnumerable<ExistingZone> existingZones)
        {
            CloseRatesForClosedZones(existingZones);
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingRatesByZoneName existingRatesByZoneName = StructureExistingRatesByZoneName(existingRates);
            foreach(var importedRate in importedRates)
            {
                List<NewRate> ratesToAdd = new List<NewRate>();
                List<ExistingRate> matchExistingRates;
                if (existingRatesByZoneName.TryGetValue(importedRate.ZoneName, out matchExistingRates))
                {
                    bool shouldNotAddRate;
                    ExistingRate recentExistingRate;
                    CloseExistingOverlapedRates(importedRate, matchExistingRates, out shouldNotAddRate, out recentExistingRate);
                    if (!shouldNotAddRate)
                    {
                        if (recentExistingRate != null)
                        {
                            if (importedRate.NormalRate > recentExistingRate.RateEntity.NormalRate)
                                importedRate.ChangeType = RateChangeType.Increase;
                            else if (importedRate.NormalRate < recentExistingRate.RateEntity.NormalRate)
                                importedRate.ChangeType = RateChangeType.Decrease;

                            importedRate.ProcessInfo.RecentExistingRate = recentExistingRate;
                        }
                        else
                        {
                            importedRate.ChangeType = RateChangeType.New;
                        }
                        AddImportedRate(importedRate, newAndExistingZones, existingZonesByName);
                    }
                }
                else
                {
                    importedRate.ChangeType = RateChangeType.New;
                    AddImportedRate(importedRate, newAndExistingZones, existingZonesByName);
                }
            }
        }

        private void CloseRatesForClosedZones(IEnumerable<ExistingZone> existingZones)
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
                                }
                                DateTime rateBED = existingRate.RateEntity.BED;
                                existingRate.ChangedRate.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }

        private void CloseExistingOverlapedRates(ImportedRate importedRate, List<ExistingRate> matchExistingRates, out bool shouldNotAddRate, out ExistingRate recentExistingRate)
        {
            shouldNotAddRate = false;
            recentExistingRate = null;
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.RateEntity.BED <= importedRate.BED)
                    recentExistingRate = existingRate;
                if (existingRate.IsOverlappedWith(importedRate))
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
                        DateTime existingRateEED = Utilities.Max(importedRate.BED, existingRate.BED);
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
                if (zone.EED.VRGreaterThan(zone.BED) && zone.EED.VRGreaterThan(currentRateBED) && importedRate.EED.VRGreaterThan(zone.BED))
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
                NormalRate =(decimal) importedRate.NormalRate,
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
            return importedRate.BED == existingRate.RateEntity.BED
               && importedRate.NormalRate == existingRate.RateEntity.NormalRate
               && importedRate.CurrencyId == existingRate.RateEntity.CurrencyId
               //TODO: compare CurrencyId of the Pricelists
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
