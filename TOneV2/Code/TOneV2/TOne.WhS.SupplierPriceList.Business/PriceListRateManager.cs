using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(IProcessCountryRatesContext context, IEnumerable<int> importedRateTypeIds)
        {
            ProcessCountryRates(context.ImportedZones, context.ExistingRatesGroupsByZoneName, context.NewAndExistingZones, context.ExistingZones, context.PriceListDate, importedRateTypeIds);
            context.NewRates = context.ImportedZones.SelectMany(item => item.NormalRate.NewRates).Union(context.ImportedZones.SelectMany(itm => itm.OtherRates.SelectMany(x => x.Value.NewRates)));
            context.ChangedRates = context.ExistingZones.SelectMany(item => item.ExistingRates.Where(itm => itm.ChangedRate != null).Select(x => x.ChangedRate));
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

        private void ProcessCountryRates(IEnumerable<ImportedZone> importedZones, Dictionary<string, ExistingRateGroup> existingRatesGroupsByZoneName, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, DateTime pricelistDate, IEnumerable<int> importedRateTypeIds)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            foreach (ImportedZone importedZone in importedZones)
            {
                ExistingRateGroup existingRateGroup;
                existingRatesGroupsByZoneName.TryGetValue(importedZone.ZoneName, out existingRateGroup);

                ProcessCountryNormalRates(importedZone.NormalRate, existingRateGroup, newAndExistingZones, existingZonesByName);
                ProcessCountryOtherRates(importedZone.OtherRates, existingRateGroup, newAndExistingZones, existingZonesByName);
                if (existingRateGroup != null)
                {
                    IEnumerable<ExistingRate> existingOtherRatesToClose = GetExistingOtherRatesToClose(existingRateGroup.OtherRates, importedZone.OtherRates, importedRateTypeIds);
                    CloseNotImportedOtherRates(existingOtherRatesToClose, pricelistDate);
                    FillNotImportedRatesForImportedZone(importedZone, existingOtherRatesToClose);
                    FillNotImportedRates(importedZone, existingRateGroup.OtherRates, importedRateTypeIds);
                }
            }


            CloseRatesForClosedZones(existingZones);
        }

        private void FillNotImportedRates(ImportedZone importedZone, Dictionary<int, List<ExistingRate>> existingOtherRates, IEnumerable<int> importedRateTypeIds)
        {
            foreach (KeyValuePair<int, List<ExistingRate>> item in existingOtherRates)
            {
                if (!importedRateTypeIds.Contains(item.Key))
                {
                    foreach (ExistingRate existingRate in item.Value)
                    {
                        NotImportedRate notImportedRate = new NotImportedRate()
                        {
                            BED = existingRate.BED,
                            EED = existingRate.EED,
                            SystemRate = existingRate.RateEntity.NormalRate,
                            HasChanged = false,
                            RateTypeId = existingRate.RateEntity.RateTypeId
                        };
                        importedZone.NotImportedRates.Add(notImportedRate);
                    }
                }
            }
        }

        private void FillNotImportedRatesForImportedZone(ImportedZone importedZone, IEnumerable<ExistingRate> existingOtherRatesToClose)
        {
            foreach (ExistingRate existingRateToClose in existingOtherRatesToClose)
            {
                NotImportedRate notImportedRate = new NotImportedRate()
                {
                    BED = existingRateToClose.BED,
                    EED = existingRateToClose.EED,
                    SystemRate = existingRateToClose.RateEntity.NormalRate,
                    HasChanged = true,
                    RateTypeId = existingRateToClose.RateEntity.RateTypeId
                };
                importedZone.NotImportedRates.Add(notImportedRate);
            }
        }

        private void ProcessCountryNormalRates(ImportedRate importedRate, ExistingRateGroup existingRateGroup, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            List<ExistingRate> existingNormalRates = existingRateGroup != null ? existingRateGroup.NormalRates : null;
            ProcessImportedRate(importedRate, existingNormalRates, newAndExistingZones, existingZonesByName);
        }

        private void ProcessCountryOtherRates(Dictionary<int, ImportedRate> importedOtherRates, ExistingRateGroup existingRateGroup, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            List<ExistingRate> existingRates = null;
            foreach (KeyValuePair<int, ImportedRate> item in importedOtherRates)
            {
                if (existingRateGroup != null)
                    existingRateGroup.OtherRates.TryGetValue(item.Key, out existingRates);

                ProcessImportedRate(item.Value, existingRates, newAndExistingZones, existingZonesByName);
            }
        }

        private void ProcessImportedRate(ImportedRate importedRate, List<ExistingRate> matchExistingRates, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName)
        {
            if (matchExistingRates != null && matchExistingRates.Count() > 0)
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

        private IEnumerable<ExistingRate> GetExistingOtherRatesToClose(Dictionary<int, List<ExistingRate>> existingOtherRates, Dictionary<int, ImportedRate> importedOtherRates, IEnumerable<int> importedRateTypeIds)
        {
            List<ExistingRate> existingOtherRatesToClose = new List<ExistingRate>();
            List<ExistingRate> matchExistingOtherRates;
            foreach (int importedRateTypeId in importedRateTypeIds)
            {
                if (!importedOtherRates.ContainsKey(importedRateTypeId))
                {
                    if (existingOtherRates.TryGetValue(importedRateTypeId, out matchExistingOtherRates))
                        existingOtherRatesToClose.AddRange(matchExistingOtherRates);
                }
            }

            return existingOtherRatesToClose;
        }

        private void CloseNotImportedOtherRates(IEnumerable<ExistingRate> existingOtherRates, DateTime codeCloseDate)
        {
            foreach (var existingOtherRate in existingOtherRates)
            {
                //Get max between BED and Close Date to avoid closing a code with EED before BED
                DateTime? closureDate = Utilities.Max(codeCloseDate, existingOtherRate.BED);
                if (!existingOtherRate.RateEntity.EED.HasValue && closureDate.VRLessThan(existingOtherRate.EED))
                {
                    //Only in this case closing has a meaning, otherwise no need to close the code
                    existingOtherRate.ChangedRate = new ChangedRate
                    {
                        EntityId = existingOtherRate.RateEntity.SupplierRateId,
                        EED = closureDate.Value
                    };
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
                                        EntityId = existingRate.RateEntity.SupplierRateId
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
                        else if (importedRate.EED.HasValue && importedRate.EED.VRLessThan(existingRate.EED))
                        {
                            existingRate.ChangedRate = new ChangedRate
                            {
                                EntityId = existingRate.RateEntity.SupplierRateId,
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
                            EntityId = existingRate.RateEntity.SupplierRateId,
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
                NormalRate = (decimal)importedRate.NormalRate,
                OtherRates = importedRate.OtherRates,
                RateTypeId = importedRate.RateTypeId,
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

            foreach (var importedRateEntry in importedRate.OtherRates)
            {
                Decimal matchExistingRate;
                if (!existingRate.RateEntity.OtherRates.TryGetValue(importedRateEntry.Key, out matchExistingRate) || importedRateEntry.Value != matchExistingRate)
                    return false;
            }
            return true;
        }

    }
}
