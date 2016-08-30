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
            ProcessCountryRates(context.ImportedZones, context.ExistingRatesGroupsByZoneName, context.NewAndExistingZones, context.ExistingZones, context.PriceListDate, importedRateTypeIds, context.NotImportedZones);
            context.NewRates = context.ImportedZones.SelectMany(item => item.ImportedNormalRate.NewRates).Union(context.ImportedZones.SelectMany(itm => itm.ImportedOtherRates.SelectMany(x => x.Value.NewRates)));
            context.ChangedRates = context.ExistingZones.SelectMany(item => item.ExistingRates.Where(itm => itm.ChangedRate != null).Select(x => x.ChangedRate));
        }

        private void ProcessCountryRates(IEnumerable<ImportedZone> importedZones, Dictionary<string, ExistingRateGroup> existingRatesGroupsByZoneName, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, DateTime pricelistDate, IEnumerable<int> importedRateTypeIds, IEnumerable<NotImportedZone> notImportedZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);

            ProcessImportedData(importedZones, newAndExistingZones, existingZonesByName, existingRatesGroupsByZoneName, importedRateTypeIds, pricelistDate);

            ProcessNotImportedData(existingZones, notImportedZones, existingRatesGroupsByZoneName);
        }


        #region Processing Imported Data Methods

        private void ProcessImportedData(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName,
            Dictionary<string, ExistingRateGroup> existingRatesGroupsByZoneName, IEnumerable<int> importedRateTypeIds, DateTime pricelistDate)
        {
            foreach (ImportedZone importedZone in importedZones)
            {
                ExistingRateGroup existingRateGroup;
                existingRatesGroupsByZoneName.TryGetValue(importedZone.ZoneName, out existingRateGroup);

                Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId = new Dictionary<int, List<ExistingRate>>();

                ProcessData(importedZone, newAndExistingZones, existingZonesByName, existingRateGroup, importedRateTypeIds, pricelistDate, existingOtherRatesToCloseByRateTypeId);
                PrepareDataForPreview(importedZone, existingRateGroup, importedRateTypeIds, existingOtherRatesToCloseByRateTypeId);
            }
        }

        private void ProcessData(ImportedZone importedZone, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName, ExistingRateGroup existingRateGroup,
            IEnumerable<int> importedRateTypeIds, DateTime pricelistDate, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            ProcessCountryNormalRates(importedZone.ImportedNormalRate, existingRateGroup, newAndExistingZones, existingZonesByName);
            ProcessCountryOtherRates(importedZone.ImportedOtherRates, existingRateGroup, newAndExistingZones, existingZonesByName);
            if (existingRateGroup != null)
            {
                GetExistingOtherRatesToClose(existingRateGroup.OtherRates, importedZone.ImportedOtherRates, importedRateTypeIds, existingOtherRatesToCloseByRateTypeId);
                List<ExistingRate> existingOtherRateToClose = existingOtherRatesToCloseByRateTypeId.Values.SelectMany(item => item).ToList();
                CloseNotImportedOtherRates(existingOtherRateToClose, pricelistDate);
            }
        }

        private void PrepareDataForPreview(ImportedZone importedZone, ExistingRateGroup existingRateGroup, IEnumerable<int> importedRateTypeIds, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            FillSystemRateForImportedZone(importedZone);
            if (existingRateGroup != null)
            {
                FillNotImportedRatesForImportedZone(importedZone, existingOtherRatesToCloseByRateTypeId);
                FillNotImportedRates(importedZone, existingRateGroup.OtherRates, importedRateTypeIds);
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

        private void CloseNotImportedOtherRates(IEnumerable<ExistingRate> existingOtherRates, DateTime rateCloseDate)
        {
            foreach (var existingOtherRate in existingOtherRates)
            {
                //Get max between BED and Close Date to avoid closing a rate with EED before BED
                DateTime? closureDate = Utilities.Max(rateCloseDate, existingOtherRate.BED);
                if (!existingOtherRate.RateEntity.EED.HasValue && closureDate.VRLessThan(existingOtherRate.EED))
                {
                    //Only in this case closing has a meaning, otherwise no need to close the rate
                    existingOtherRate.ChangedRate = new ChangedRate
                    {
                        EntityId = existingOtherRate.RateEntity.SupplierRateId,
                        EED = closureDate.Value
                    };
                }
            }
        }

        private void FillSystemRateForImportedZone(ImportedZone importedZone)
        {
            if (importedZone.ExistingZones == null || importedZone.ExistingZones.Count() == 0)
                return;

            List<ExistingZone> connectedExistingZones = importedZone.ExistingZones.GetConnectedEntities(DateTime.Today);
            importedZone.ImportedNormalRate.SystemRate = GetSystemNormalRate(importedZone, connectedExistingZones);
            foreach (ImportedRate importedOtherRate in importedZone.ImportedOtherRates.Values)
            {
                importedOtherRate.SystemRate = GetSystemOtherRate(importedOtherRate, connectedExistingZones);
            }
        }

        private void FillNotImportedRatesForImportedZone(ImportedZone importedZone, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            foreach (KeyValuePair<int, List<ExistingRate>> item in existingOtherRatesToCloseByRateTypeId)
            {
                ExistingRate existingOtherRateToClose = item.Value.GetConnectedEntities(DateTime.Now).LastOrDefault();
                NotImportedRate notImportedRate = new NotImportedRate()
                {
                    BED = existingOtherRateToClose.BED,
                    EED = existingOtherRateToClose.EED,
                    Rate = existingOtherRateToClose.RateEntity.NormalRate,
                    HasChanged = true,
                    RateTypeId = item.Key
                };
                importedZone.NotImportedOtherRates.Add(notImportedRate);
            }
        }

        private void FillNotImportedRates(ImportedZone importedZone, Dictionary<int, List<ExistingRate>> existingOtherRates, IEnumerable<int> importedRateTypeIds)
        {
            foreach (KeyValuePair<int, List<ExistingRate>> item in existingOtherRates)
            {
                IEnumerable<ExistingRate> connectedExistingOtherRates = item.Value.GetConnectedEntities(DateTime.Today);
                if (!importedRateTypeIds.Contains(item.Key))
                {
                    NotImportedRate notImportedRate = new NotImportedRate()
                    {
                        BED = connectedExistingOtherRates.LastOrDefault().BED,
                        EED = connectedExistingOtherRates.LastOrDefault().EED,
                        Rate = connectedExistingOtherRates.LastOrDefault().RateEntity.NormalRate,
                        HasChanged = false,
                        RateTypeId = item.Key
                    };
                    importedZone.NotImportedOtherRates.Add(notImportedRate);
                }
            }
        }

        private ExistingRate GetSystemNormalRate(ImportedZone importedZone, IEnumerable<ExistingZone> connectedExistingZones)
        {
            if (importedZone.ImportedNormalRate.ProcessInfo.RecentExistingRate != null)
                return importedZone.ImportedNormalRate.ProcessInfo.RecentExistingRate;

            IEnumerable<ExistingRate> existingNormalRates = connectedExistingZones.SelectMany(item => item.ExistingRates).FindAllRecords(itm => !itm.RateEntity.RateTypeId.HasValue);
            return existingNormalRates.LastOrDefault();
        }

        private ExistingRate GetSystemOtherRate(ImportedRate importedOtherRate, IEnumerable<ExistingZone> connectedExistingZones)
        {
            if (importedOtherRate.ProcessInfo.RecentExistingRate != null)
                return importedOtherRate.ProcessInfo.RecentExistingRate;

            IEnumerable<ExistingRate> existingNormalRates = connectedExistingZones.SelectMany(item => item.ExistingRates.Where(existingRate => existingRate.RateEntity.RateTypeId.HasValue && existingRate.RateEntity.RateTypeId == importedOtherRate.RateTypeId));
            return existingNormalRates.LastOrDefault();

        }

        private void GetExistingOtherRatesToClose(Dictionary<int, List<ExistingRate>> existingOtherRates, Dictionary<int, ImportedRate> importedOtherRates,
           IEnumerable<int> importedRateTypeIds, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            List<ExistingRate> matchExistingOtherRates;

            foreach (int importedRateTypeId in importedRateTypeIds)
            {
                if (!importedOtherRates.ContainsKey(importedRateTypeId))
                {
                    if (existingOtherRates.TryGetValue(importedRateTypeId, out matchExistingOtherRates))
                    {
                        if (existingOtherRatesToCloseByRateTypeId.ContainsKey(importedRateTypeId))
                            existingOtherRatesToCloseByRateTypeId[importedRateTypeId].AddRange(matchExistingOtherRates);
                        else
                            existingOtherRatesToCloseByRateTypeId.Add(importedRateTypeId, matchExistingOtherRates);
                    }
                }
            }
        }

        #endregion


        #region Prcessing Not Imported Data Methods

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones, Dictionary<string, ExistingRateGroup> existingRatesGroupsByZoneName)
        {
            //Make sure that Closing Rates for closed zones must be done before filling other sytem rates of not imported zones
            CloseRatesForClosedZones(existingZones);
            FillRatesForNotImportedZones(notImportedZones, existingRatesGroupsByZoneName, existingZones);
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

        private void FillRatesForNotImportedZones(IEnumerable<NotImportedZone> notImportedZones, Dictionary<string, ExistingRateGroup> existingRatesGroupsByZoneName, IEnumerable<ExistingZone> existingZones)
        {
            ExistingRateGroup existingRateGroup;

            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                IEnumerable<ExistingZone> connectedExistingZones = existingZones.FindAllRecords(item => item.Name.Equals(notImportedZone.ZoneName, StringComparison.InvariantCultureIgnoreCase)).ToList().GetConnectedEntities(DateTime.Today);
                ExistingRate existingNormalRate = connectedExistingZones.SelectMany(item => item.ExistingRates).FindAllRecords(itm => !itm.RateEntity.RateTypeId.HasValue).LastOrDefault();

                notImportedZone.NormalRate = NotImportedRateMapper(existingNormalRate);

                List<ExistingRate> existingOtherRates = new List<ExistingRate>();
                if (existingRatesGroupsByZoneName.TryGetValue(notImportedZone.ZoneName, out existingRateGroup))
                {
                    foreach (KeyValuePair<int, List<ExistingRate>> item in existingRateGroup.OtherRates)
                    {
                        existingOtherRates.AddRange(item.Value);
                    }
                }

                notImportedZone.OtherRates.AddRange(existingOtherRates.MapRecords(NotImportedRateMapper));
            }
        }

        #endregion

        
        #region Private Methods

        private NotImportedRate NotImportedRateMapper(ExistingRate existingRate)
        {
            return new NotImportedRate()
            {
                BED = existingRate.BED,
                EED = existingRate.EED,
                Rate = existingRate.RateEntity.NormalRate,
                RateTypeId = existingRate.RateEntity.RateTypeId,
                HasChanged = true
            };
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
                        if (importedRate.Rate > recentExistingRate.RateEntity.NormalRate)
                            importedRate.ChangeType = RateChangeType.Increase;
                        else if (importedRate.Rate < recentExistingRate.RateEntity.NormalRate)
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
                NormalRate = (decimal)importedRate.Rate,
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
               && importedRate.Rate == existingRate.RateEntity.NormalRate
               && importedRate.CurrencyId == existingRate.RateEntity.CurrencyId;
            //TODO: compare CurrencyId of the Pricelists

        }


        #endregion
      
       
        //private bool SameRateOtherRates(ImportedRate importedRate, ExistingRate existingRate)
        //{
        //    int importedRatesCount = importedRate.OtherRates != null ? importedRate.OtherRates.Count : 0;
        //    int existingRatesCount = existingRate.RateEntity.OtherRates != null ? existingRate.RateEntity.OtherRates.Count : 0;
        //    if (importedRatesCount != existingRatesCount)
        //        return false;
        //    if (importedRatesCount == 0)
        //        return true;
        //    //if rates Count is > 0, then both dictionaries are not null

        //    foreach (var importedRateEntry in importedRate.OtherRates)
        //    {
        //        Decimal matchExistingRate;
        //        if (!existingRate.RateEntity.OtherRates.TryGetValue(importedRateEntry.Key, out matchExistingRate) || importedRateEntry.Value != matchExistingRate)
        //            return false;
        //    }
        //    return true;
        //}

    }
}
