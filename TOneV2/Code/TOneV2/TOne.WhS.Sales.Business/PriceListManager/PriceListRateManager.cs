using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager
    {
        public void ProcessCountryRates(IProcessRatesContext context)
        {
            var newExplicitRates = new List<NewRate>();
            ProcessRates(context.RatesToChange, context.RatesToClose, context.ExistingRates, context.ExistingZones, context.ExplicitlyChangedExistingCustomerCountries, context.InheritedRatesByZoneId, newExplicitRates);

            List<NewRate> newRates = new List<NewRate>();
            newRates.AddRange(context.RatesToChange.SelectMany(x => x.NewRates));
            newRates.AddRange(newExplicitRates);

            context.NewRates = newRates;
            context.ChangedRates = context.ExistingRates.Where(x => x.ChangedRate != null).Select(x => x.ChangedRate);
        }

        #region Private Methods

        private void ProcessRates(IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose, IEnumerable<ExistingRate> existingRates, IEnumerable<ExistingZone> existingZones, IEnumerable<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries, InheritedRatesByZoneId inheritedRatesByZoneId, List<NewRate> newExplicitRates)
        {
            Dictionary<int, List<ExistingZone>> existingZonesByCountry;
            ExistingZonesByName existingZonesByName;
            StructureExistingZonesByCountryAndName(existingZones, out existingZonesByCountry, out existingZonesByName);

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
                        rateToChange.ChangeType = RateChangeType.New;

                        if (recentExistingRate != null)
                        {
                            if (rateToChange.NormalRate > recentExistingRate.ConvertedRate)
                                rateToChange.ChangeType = RateChangeType.Increase;
                            else if (rateToChange.NormalRate < recentExistingRate.ConvertedRate)
                                rateToChange.ChangeType = RateChangeType.Decrease;
                            else rateToChange.ChangeType = RateChangeType.NotChanged;
                            rateToChange.RecentExistingRate = recentExistingRate;
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

            if (explicitlyChangedExistingCustomerCountries.Count() > 0)
            {
                Dictionary<int, CountryRange> endedCountryRangesByCountryId = GetEndedCountryRangesByCountryId(explicitlyChangedExistingCustomerCountries);

                var countryManager = new Vanrise.Common.Business.CountryManager();

                foreach (ExistingCustomerCountry changedExistingCountry in explicitlyChangedExistingCustomerCountries)
                {
                    int countryId = changedExistingCountry.CustomerCountryEntity.CountryId;
                    string countryName = countryManager.GetCountryName(countryId);

                    List<ExistingZone> matchedExistingZones = existingZonesByCountry.GetRecord(countryId);
                    if (matchedExistingZones == null || matchedExistingZones.Count == 0)
                        throw new DataIntegrityValidationException(string.Format("No existing zones for country '{0}' were found", countryName));

                    CountryRange countryRange = endedCountryRangesByCountryId.GetRecord(countryId);
                    if (countryRange == null)
                        throw new DataIntegrityValidationException(string.Format("The BED of country '{0}' was not found", countryName));
                        ProcessChangedExistingCountry(changedExistingCountry, matchedExistingZones, inheritedRatesByZoneId, countryRange, newExplicitRates);
                }
            }
        }

        private void StructureExistingZonesByCountryAndName(IEnumerable<ExistingZone> existingZones, out Dictionary<int, List<ExistingZone>> existingZonesByCountry, out ExistingZonesByName existingZonesByName)
        {
            existingZonesByCountry = new Dictionary<int, List<ExistingZone>>();
            existingZonesByName = new ExistingZonesByName();

            List<ExistingZone> zones;

            foreach (ExistingZone existingZone in existingZones)
            {
                if (!existingZonesByCountry.TryGetValue(existingZone.CountryId, out zones))
                {
                    zones = new List<ExistingZone>();
                    existingZonesByCountry.Add(existingZone.CountryId, zones);
                }
                zones.Add(existingZone);

                if (!existingZonesByName.TryGetValue(existingZone.Name, out zones))
                {
                    zones = new List<ExistingZone>();
                    existingZonesByName.Add(existingZone.Name, zones);
                }
                zones.Add(existingZone);
            }
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
                Rate = rateToChange.NormalRate,
                Zone = zone,
                BED = zone.BED > currentRateBED ? zone.BED : currentRateBED,
                EED = rateToChange.EED,
                ChangeType = rateToChange.ChangeType
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

        #region Process Changed Existing Countries

        private void ProcessChangedExistingCountry(ExistingCustomerCountry changedExistingCountry, IEnumerable<ExistingZone> matchedExistingZones, InheritedRatesByZoneId inheritedRatesByZoneId, CountryRange countryRange, List<NewRate> newExplicitRates)
        {
            foreach (ExistingZone existingZone in matchedExistingZones)
            {
                foreach (ExistingRate existingRate in existingZone.ExistingRates)
                {
                    if (existingRate.EED.VRGreaterThan(changedExistingCountry.ChangedCustomerCountry.EED))
                    {
                        existingRate.ChangedRate = new ChangedRate()
                        {
                            RateId = existingRate.RateEntity.SaleRateId,
                            EED = Vanrise.Common.Utilities.Max(existingRate.BED, changedExistingCountry.ChangedCustomerCountry.EED)
                        };
                    }
                }
                if (countryRange.EED.VRGreaterThan(countryRange.BED) && existingZone.BED < changedExistingCountry.EED.Value)
                    AddZoneExplicitRates(existingZone, inheritedRatesByZoneId.GetRecord(existingZone.ZoneId), countryRange, newExplicitRates);
            }
        }

        private Dictionary<int, CountryRange> GetEndedCountryRangesByCountryId(IEnumerable<ExistingCustomerCountry> changedExistingCountries)
        {
            var endedCountryRangesByCountryId = new Dictionary<int, CountryRange>();

            foreach (ExistingCustomerCountry endedCountry in changedExistingCountries)
            {
                int endedCountryId = endedCountry.CustomerCountryEntity.CountryId;

                if (!endedCountryRangesByCountryId.ContainsKey(endedCountryId))
                {
                    endedCountryRangesByCountryId.Add(endedCountryId, new CountryRange()
                    {
                        BED = Utilities.Max(endedCountry.BED, DateTime.Today),
                        EED = endedCountry.EED
                    });
                }
            }

            return endedCountryRangesByCountryId;
        }

        private void AddZoneExplicitRates(ExistingZone existingZone, ZoneInheritedRates zoneInheritedRates, CountryRange countryRange, List<NewRate> newExplicitRates)
        {
            if (zoneInheritedRates == null)
                throw new DataIntegrityValidationException(string.Format("No inherited rates were found for zone '{0}'", existingZone.Name));

            // Step 1: Prepare the inherited normal rates

            if (zoneInheritedRates.NormalRates == null || zoneInheritedRates.NormalRates.Count == 0)
                throw new DataIntegrityValidationException(string.Format("No inherited normal rates were found for zone '{0}'", existingZone.Name));

            Action<SaleRate, Rate> mapSaleRate = (saleRate, rate) =>
            {
                rate.RateTypeId = saleRate.RateTypeId;
                rate.RateValue = saleRate.Rate;
                rate.ZoneId = saleRate.ZoneId;
                rate.CurrencyId = saleRate.CurrencyId;
                rate.PriceListId = saleRate.PriceListId;
                rate.RateChange = saleRate.RateChange;
                rate.Source = SalePriceListOwnerType.SellingProduct;
            };
            var countryRangeAsList = new List<CountryRange>() { countryRange };
            IEnumerable<Rate> inheritedNormalRates = Utilities.GetQIntersectT<CountryRange, SaleRate, Rate>(countryRangeAsList, zoneInheritedRates.NormalRates, mapSaleRate);

            if (inheritedNormalRates == null || inheritedNormalRates.Count() == 0)
                throw new DataIntegrityValidationException(string.Format("No inherited normal rates were found for zone '{0}'", existingZone.Name));

            // Step 2: Prepare the existing explicit normal rates

            Func<ExistingRate, Rate> mapExistingRate = (existingRate) =>
            {
                return new Rate()
                {
                    RateTypeId = existingRate.RateEntity.RateTypeId,
                    RateValue = existingRate.RateEntity.Rate,
                    ZoneId = existingRate.RateEntity.ZoneId,
                    CurrencyId = existingRate.RateEntity.CurrencyId,
                    PriceListId = existingRate.RateEntity.PriceListId,
                    BED = existingRate.BED,
                    EED = existingRate.EED,
                    RateChange = existingRate.RateEntity.RateChange,
                    Source = SalePriceListOwnerType.Customer
                };
            };
            IEnumerable<Rate> explicitNormalRates = existingZone.ExistingRates.MapRecords(mapExistingRate, x => !x.RateEntity.RateTypeId.HasValue);

            // Step 3: Merge the inherited normal rates with the existing explicit normal ones

            Action<Rate, Rate> mapRate = (rate, rRate) =>
            {
                rRate.RateTypeId = rate.RateTypeId;
                rRate.RateValue = rate.RateValue;
                rRate.ZoneId = rate.ZoneId;
                rRate.CurrencyId = rate.CurrencyId;
                rRate.PriceListId = rate.PriceListId;
                rRate.RateChange = rate.RateChange;
                rRate.Source = rate.Source;
            };
            IEnumerable<Rate> mergedNormalRates = Utilities.MergeUnionWithQForce(inheritedNormalRates.ToList(), explicitNormalRates.ToList(), mapRate, mapRate);

            // Step 4: Create new explicit normal rates from the inherited normal ones that have resulted from the merge process

            foreach (Rate mergedNormalRate in mergedNormalRates)
            {
                if (mergedNormalRate.Source == SalePriceListOwnerType.SellingProduct)
                {
                    newExplicitRates.Add(new NewRate()
                    {
                        RateId = 0,
                        Zone = existingZone,
                        RateTypeId = mergedNormalRate.RateTypeId,
                        Rate = mergedNormalRate.RateValue,
                        CurrencyId = mergedNormalRate.CurrencyId,
                        BED = mergedNormalRate.BED,
                        EED = mergedNormalRate.EED,
                        ChangeType = mergedNormalRate.RateChange,
                    });
                }
            }
        }

        private class CountryRange : Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }

        private class Rate : Vanrise.Entities.IDateEffectiveSettings, Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public int? RateTypeId { get; set; }
            public decimal RateValue { get; set; }
            public long ZoneId { get; set; }
            public int? CurrencyId { get; set; }
            public int PriceListId { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
            public RateChangeType RateChange { get; set; }
            public SalePriceListOwnerType Source { get; set; }
        }

        #endregion

        #endregion
    }
}
