using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager
    {
        #region Public Methods

        public void ProcessCountryRates(IProcessRatesContext context)
        {
            ProcessRates(context.RatesToChange, context.RatesToClose, context.ExistingRates, context.ExistingZones);
            context.NewRates = context.RatesToChange.SelectMany(itm => itm.NewRates);
            context.ChangedRates = context.ExistingRates.Where(itm => itm.ChangedRate != null).Select(itm => itm.ChangedRate);
        }

        #endregion

        #region Private Methods

        private void ProcessRates(IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose, IEnumerable<ExistingRate> existingRates, IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);
            ExistingRatesByZoneName existingRatesByZoneName = StructureExistingRatesByZoneName(existingRates);

            foreach (RateToChange rateToChange in ratesToChange)
            {
                IEnumerable<ExistingRate> matchedExistingRates = GetMatchedExistingRates(existingRatesByZoneName, rateToChange.ZoneName, rateToChange.RateTypeId);
                if (matchedExistingRates != null)
                {
                    bool shouldNotAddRate;
                    ExistingRate recentExistingRate;
                    CloseExistingOverlappedRates(rateToChange, matchedExistingRates, out shouldNotAddRate, out recentExistingRate);
                    if (!shouldNotAddRate)
                    {
                        if (recentExistingRate != null)
                        {
                            if (rateToChange.NormalRate > recentExistingRate.ConvertedRate)
                                rateToChange.ChangeType = RateChangeType.Increase;
                            else if (rateToChange.NormalRate < recentExistingRate.ConvertedRate)
                                rateToChange.ChangeType = RateChangeType.Decrease;

                            rateToChange.RecentExistingRate = recentExistingRate;
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
                IEnumerable<ExistingRate> matchExistingRates = GetMatchedExistingRates(existingRatesByZoneName, rateToClose.ZoneName, rateToClose.RateTypeId);
                if (matchExistingRates != null)
                    CloseExistingRates(rateToClose, matchExistingRates);
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

        private ExistingRatesByZoneName StructureExistingRatesByZoneName(IEnumerable<ExistingRate> existingRates)
        {
            ExistingRatesByZoneName existingRatesByZoneName = new ExistingRatesByZoneName();
            List<ExistingRate> existingRatesList = null;

            if (existingRates != null)
            {
                foreach (ExistingRate item in existingRates)
                {
                    if (!existingRatesByZoneName.TryGetValue(item.ParentZone.Name, out existingRatesList))
                    {
                        existingRatesList = new List<ExistingRate>();
                        existingRatesByZoneName.Add(item.ParentZone.Name, existingRatesList);
                    }

                    existingRatesList.Add(item);
                }
            }

            return existingRatesByZoneName;
        }

        private IEnumerable<ExistingRate> GetMatchedExistingRates(ExistingRatesByZoneName ratesByZone, string zoneName, int? rateTypeId)
        {
            List<ExistingRate> matchedRates;
            if (ratesByZone.TryGetValue(zoneName, out matchedRates))
            {
                return matchedRates.FindAllRecords
                (
                    x => (!rateTypeId.HasValue && !x.RateEntity.RateTypeId.HasValue) ||
                    (rateTypeId.HasValue && x.RateEntity.RateTypeId.HasValue && x.RateEntity.RateTypeId.Value == rateTypeId.Value)
                );
            }
            return matchedRates;
        }

        #region Process Rates To Change

        private void CloseExistingOverlappedRates(RateToChange rateToChange, IEnumerable<ExistingRate> matchExistingRates, out bool shouldNotAddRate, out ExistingRate recentExistingRate)
        {
            shouldNotAddRate = false;
            recentExistingRate = null;
            foreach (var existingRate in matchExistingRates)
            {
                if (existingRate.RateEntity.BED <= rateToChange.BED)
                    recentExistingRate = existingRate;
                if (existingRate.IsOverlappedWith(rateToChange))
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
                RateTypeId = rateToChange.RateTypeId,
                NormalRate = rateToChange.NormalRate,
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

        #endregion

        #region Process Rates To Close

        private void CloseExistingRates(RateToClose rateToClose, IEnumerable<ExistingRate> matchExistingRates)
        {
            foreach (var existingRate in matchExistingRates.OrderBy(x => x.BED))
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

        #endregion

        #endregion
    }
}
