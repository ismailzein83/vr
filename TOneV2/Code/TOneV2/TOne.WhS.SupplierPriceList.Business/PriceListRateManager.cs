using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class PriceListRateManager
    {
        public void ProcessRates(List<ImportedRate> importedRates, List<NewRate> newRates, ZonesByName newAndExistingZones, ExistingZonesByName existingZones, ExistingRatesByZoneName existingRates, List<ChangedRate> changedRates)
        {
            foreach(var importedRate in importedRates)
            {
                List<NewRate> ratesToAdd = new List<NewRate>();
                List<ExistingRate> matchExistingRates;
                if(existingRates.TryGetValue(importedRate.ZoneName, out matchExistingRates))
                {
                    bool shouldNotAddRate;
                    CloseExistingOverlapedRates(importedRate, matchExistingRates, changedRates, out shouldNotAddRate);
                    if (!shouldNotAddRate)
                    {
                        AddImportedRate(importedRate, newRates, newAndExistingZones, existingZones);
                    }
                }
                else
                {
                    AddImportedRate(importedRate, newRates, newAndExistingZones, existingZones);
                }
            }
        }

        private void CloseExistingOverlapedRates(ImportedRate importedRate, List<ExistingRate> matchExistingRates, List<ChangedRate> changedRates, out bool shouldNotAddRate)
        {
            shouldNotAddRate = false;
            foreach (var existingRate in matchExistingRates)
            {
                if (!existingRate.RateEntity.EndEffectiveDate.HasValue || existingRate.RateEntity.EndEffectiveDate.Value > importedRate.BED)
                {
                    if (SameRates(importedRate, existingRate))
                    {
                        shouldNotAddRate = true;
                        break;
                    }
                    else
                    {
                        DateTime existingRateEED = importedRate.BED > existingRate.RateEntity.BeginEffectiveDate ? importedRate.BED : existingRate.RateEntity.BeginEffectiveDate;
                        changedRates.Add(new ChangedRate
                        {
                            RateId = existingRate.RateEntity.SupplierRateId,
                            EED = existingRateEED
                        });
                    }
                }
            }
        }

        private void AddImportedRate(ImportedRate importedRate, List<NewRate> newRates, ZonesByName newAndExistingZones, ExistingZonesByName existingZones)
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
                if (!zone.EED.HasValue || zone.EED.Value > currentRateBED)
                {
                    AddNewRate(importedRate, ref currentRateBED, zone, newRates, out shouldAddMoreRates);
                    if (!shouldAddMoreRates)
                        break;
                }
            }
        }

        private void AddNewRate(ImportedRate importedRate, ref DateTime currentRateBED, IZone zone, List<NewRate> newRates, out bool shouldAddMoreRates)
        {
            var newRate = new NewRate
            {
                NormalRate = importedRate.NormalRate,
                OtherRates = importedRate.OtherRates,
                 CurrencyId = importedRate.CurrencyId,
                Zone = zone,
                BED = zone.BED > currentRateBED ? zone.BED : currentRateBED,
                EED = importedRate.EED
            };
            if (zone.EED.HasValue)
            {
                if (!newRate.EED.HasValue || newRate.EED.Value > zone.EED.Value)
                    newRate.EED = zone.EED;
            }
            newRates.Add(newRate);
            if (newRate.EED == importedRate.EED)
            {
                currentRateBED = DateTime.MaxValue;
                shouldAddMoreRates = false;
            }
            else
            {
                currentRateBED = newRate.EED.Value;
                shouldAddMoreRates = true;
            }  
        }

        private bool SameRates(ImportedRate importedRate, ExistingRate existingRate)
        {
            throw new NotImplementedException();
        }
    }
}
