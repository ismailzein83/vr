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

        private void ProcessCountryRates(IEnumerable<ImportedZone> importedZones, ExistingRateGroupByZoneName existingRatesGroupsByZoneName, ZonesByName newAndExistingZones,
            IEnumerable<ExistingZone> existingZones, DateTime pricelistDate, IEnumerable<int> importedRateTypeIds, IEnumerable<NotImportedZone> notImportedZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ProcessImportedData(importedZones, newAndExistingZones, existingZonesByName, existingRatesGroupsByZoneName, importedRateTypeIds, pricelistDate);
            ProcessNotImportedData(existingZones, notImportedZones, existingRatesGroupsByZoneName);
        }


        #region Processing Imported Data Methods

        private void ProcessImportedData(IEnumerable<ImportedZone> importedZones, ZonesByName newAndExistingZones, ExistingZonesByName existingZonesByName,
            ExistingRateGroupByZoneName existingRatesGroupsByZoneName, IEnumerable<int> importedRateTypeIds, DateTime pricelistDate)
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
            FillSystemRatesForImportedZone(importedZone, existingRateGroup);
            FillNotImportedOtherRatesWithClosedRates(importedZone, existingOtherRatesToCloseByRateTypeId);
            if (existingRateGroup != null)
                FillNotImportedOtherRatesWithNotImportedRates(importedZone, existingRateGroup.OtherRates, importedRateTypeIds);
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

        private void FillSystemRatesForImportedZone(ImportedZone importedZone, ExistingRateGroup existingRateGroup)
        {
            if (existingRateGroup == null)
                return;

            importedZone.ImportedNormalRate.SystemRate = GetSystemRate(importedZone.ImportedNormalRate, existingRateGroup.NormalRates);
            foreach (ImportedRate importedOtherRate in importedZone.ImportedOtherRates.Values)
            {
                List<ExistingRate> existingOtherRates = null;
                if (existingRateGroup.OtherRates.TryGetValue(importedOtherRate.RateTypeId.Value, out existingOtherRates))
                {
                    if (existingOtherRates != null && existingOtherRates.Count > 0)
                        importedOtherRate.SystemRate = GetSystemRate(importedOtherRate, existingOtherRates);
                }
            }
        }

        private void FillNotImportedOtherRatesWithClosedRates(ImportedZone importedZone, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            foreach (KeyValuePair<int, List<ExistingRate>> item in existingOtherRatesToCloseByRateTypeId)
            {
                NotImportedRate notImportedRate = this.GetNotImportedRate(item.Value, true);
                if (notImportedRate != null)
                    importedZone.NotImportedOtherRates.Add(notImportedRate);
            }
        }

        private void FillNotImportedOtherRatesWithNotImportedRates(ImportedZone importedZone, Dictionary<int, List<ExistingRate>> existingOtherRates, IEnumerable<int> importedRateTypeIds)
        {
            foreach (KeyValuePair<int, List<ExistingRate>> item in existingOtherRates)
            {
                if (!importedRateTypeIds.Contains(item.Key))
                {
                    NotImportedRate notImportedRate = this.GetNotImportedRate(item.Value, false);
                    if (notImportedRate != null)
                        importedZone.NotImportedOtherRates.Add(notImportedRate);
                }
            }
        }

        private void GetExistingOtherRatesToClose(Dictionary<int, List<ExistingRate>> existingOtherRates, Dictionary<int, ImportedRate> importedOtherRates,
           IEnumerable<int> importedRateTypeIds, Dictionary<int, List<ExistingRate>> existingOtherRatesToCloseByRateTypeId)
        {
            List<ExistingRate> matchExistingOtherRates;

            foreach (int importedRateTypeId in importedRateTypeIds)
            {
                if (!importedOtherRates.ContainsKey(importedRateTypeId))
                {
                    existingOtherRates.TryGetValue(importedRateTypeId, out matchExistingOtherRates);
                    if (matchExistingOtherRates != null)
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

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones, ExistingRateGroupByZoneName existingRatesGroupsByZoneName)
        {
            //Make sure that Closing Rates for closed zones must be done before filling other sytem rates of not imported zones
            CloseRatesForClosedZones(existingZones);
            FillRatesForNotImportedZones(notImportedZones, existingRatesGroupsByZoneName);
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

        private void FillRatesForNotImportedZones(IEnumerable<NotImportedZone> notImportedZones, ExistingRateGroupByZoneName existingRatesGroupsByZoneName)
        {
            if (notImportedZones == null)
                return;

            ExistingRateGroup existingRateGroup;
            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                if (existingRatesGroupsByZoneName.TryGetValue(notImportedZone.ZoneName, out existingRateGroup))
                {
                    if (existingRateGroup != null)
                    {
                        NotImportedRate notImportedNormalRate = this.GetNotImportedRate(existingRateGroup.NormalRates, false);
                        if (notImportedNormalRate != null)
                            notImportedZone.NormalRate = notImportedNormalRate;
                        foreach (KeyValuePair<int, List<ExistingRate>> item in existingRateGroup.OtherRates)
                        {
                            NotImportedRate notImportedOtherRate = this.GetNotImportedRate(item.Value, false);
                            if (notImportedOtherRate != null)
                                notImportedZone.OtherRates.Add(notImportedOtherRate);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private ExistingRate GetSystemRate(ImportedRate importedRate, List<ExistingRate> existingRates)
        {
            if (importedRate.ProcessInfo.RecentExistingRate != null)
                return importedRate.ProcessInfo.RecentExistingRate;

            return GetLastExistingRateFromConnectedExistingRates(existingRates);
        }

        private NotImportedRate GetNotImportedRate(List<ExistingRate> existingRates, bool hasChanged)
        {
            ExistingRate lastElement = GetLastExistingRateFromConnectedExistingRates(existingRates);
            if (lastElement == null)
                return null;

            return new NotImportedRate()
            {
                BED = lastElement.BED,
                EED = lastElement.EED,
                Rate = lastElement.RateEntity.NormalRate,
                RateTypeId = lastElement.RateEntity.RateTypeId,
                HasChanged = hasChanged
            };
        }

        private ExistingRate GetLastExistingRateFromConnectedExistingRates(List<ExistingRate> existingRates)
        {
            List<ExistingRate> connectedExistingRates = existingRates.GetConnectedEntities(DateTime.Today);
            if (connectedExistingRates == null)
                return null;

            return connectedExistingRates.Last();
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
    }
}
