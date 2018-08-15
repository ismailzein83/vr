using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business.Reader;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class FilterChanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CustomerId { get; set; }

        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public InArgument<bool> FollowPublisherRatesBED { get; set; }

        [RequiredArgument]
        public InArgument<bool> FollowPublisherRoutingProduct { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }
        [RequiredArgument]
        public InArgument<int> PublisherId { get; set; }

        [RequiredArgument]
        public OutArgument<Changes> FilteredChanges { get; set; }

        public OutArgument<List<ExcludedChange>> ExcludedCountries { get; set; }


        protected override void Execute(CodeActivityContext context)
        {

            int customerId = this.CustomerId.Get(context);
            Changes changes = this.Changes.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            bool followPublisherRatesBED = this.FollowPublisherRatesBED.Get(context);
            bool followPublisherRoutingProduct = this.FollowPublisherRoutingProduct.Get(context);
            int publisherId = this.PublisherId.Get(context);

            List<ExcludedChange> excludedCountries = new List<ExcludedChange>();
            Changes filteredChanges = new Changes();

            if (changes.ZoneChanges != null && changes.ZoneChanges.Any())
                filteredChanges.ZoneChanges = FilterZoneChanges(customerId, changes.CountryChanges, changes.ZoneChanges, changes.CurrencyId.Value, effectiveDate, followPublisherRatesBED, followPublisherRoutingProduct, publisherId, excludedCountries);
            filteredChanges.CurrencyId = changes.CurrencyId;

            this.FilteredChanges.Set(context, filteredChanges);
            this.ExcludedCountries.Set(context, excludedCountries);
            if (excludedCountries != null && excludedCountries.Count() > 0)
            {
                foreach (var excludedCountry in excludedCountries)
                {
                    context.WriteTrackingMessage(LogEntryType.Warning, excludedCountry.Reason);
                }
            }
        }

        private List<ZoneChanges> FilterZoneChanges(int customerId, CountryChanges countryChanges, IEnumerable<ZoneChanges> zoneChanges, int currencyId, DateTime effectiveDate, bool followPublisherRatesBED, bool followPublisherRoutingProduct, int publisherId, List<ExcludedChange> excludedCountries)
        {
            var notSoldCountryIds = new List<int>();
            var countriesWithDifferentCurrencies = new List<int>();
            var carrierAccountManager = new CarrierAccountManager();
            var filteredZoneChanges = new List<ZoneChanges>();
            var countryManager = new CountryManager();
            string carrierName = carrierAccountManager.GetCarrierAccountName(customerId);
            int sellingProductId = carrierAccountManager.GetSellingProductId(customerId);
            int publisherSellingProductId = carrierAccountManager.GetSellingProductId(publisherId);
            var pricingSettings = carrierAccountManager.GetCustomerPricingSettings(customerId);
            var excludedNewCountryIds = new List<int>();
            var excludedClosedCountryIds = new List<int>();

            if (countryChanges != null)
            {
                if (countryChanges.NewCountries != null && countryChanges.NewCountries.Count() > 0)
                {
                    excludedNewCountryIds.AddRange(countryChanges.NewCountries.Select(item => item.CountryId));
                    List<string> newCountryNames = countryManager.GetCountryNames(excludedNewCountryIds);
                    newCountryNames.ThrowIfNull("countryNames");
                    excludedCountries.Add(new ExcludedChange
                        {
                            SubscriberId = customerId,
                            CountryIds = excludedNewCountryIds,
                            Reason = String.Format("Selling Country(ies): '{0}' will not be reflected to subscriber '{1}'", String.Join(" , ", newCountryNames), carrierName)
                        });
                }
                if (countryChanges.ChangedCountries != null)
                {
                    excludedClosedCountryIds.AddRange(countryChanges.ChangedCountries.Countries.Select(item => item.CountryId));
                    List<string> closedCountryNames = countryManager.GetCountryNames(excludedClosedCountryIds);
                    closedCountryNames.ThrowIfNull("countryNames");
                    excludedCountries.Add(new ExcludedChange
                    {
                        SubscriberId = customerId,
                        CountryIds = excludedClosedCountryIds,
                        Reason = String.Format("Closing Country(ies): '{0}' will not be reflected to subscriber '{1}'", String.Join(" , ", closedCountryNames), carrierName)
                    });
                }
            }

            var soldCountriesByCountryId = GetNotEndedCustomerCountriesByCountryId(customerId, effectiveDate);
            if (soldCountriesByCountryId.Count == 0)
                return filteredZoneChanges;

            TOne.WhS.BusinessEntity.Business.ConfigManager configManager = new TOne.WhS.BusinessEntity.Business.ConfigManager();

            SaleEntityZoneRateLocator cuurentRateLocator = null;
            if (!followPublisherRatesBED)
                cuurentRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveDate));

            var effectiveRateLocator = GetNewRateLocator(customerId, zoneChanges, sellingProductId); ;
            var closedRateLocator = GetClosedRateLocator(customerId, zoneChanges, sellingProductId);
            var zoneRoutingProductLocator = GetZoneRoutingProductLocator(customerId, zoneChanges);
            var zoneNewRateRoutingProductLocator = GetZoneNewRateRoutingProductLocator(customerId, publisherId, zoneChanges);

            foreach (var zoneChange in zoneChanges)
            {
                if (excludedNewCountryIds.Contains(zoneChange.CountryId))
                    continue;
                if (excludedClosedCountryIds.Contains(zoneChange.CountryId))
                    continue;
                CustomerCountry2 customerCountry;

                if (!soldCountriesByCountryId.TryGetValue(zoneChange.CountryId, out customerCountry))
                {
                    if (!notSoldCountryIds.Any(x => x == zoneChange.CountryId))
                        notSoldCountryIds.Add(zoneChange.CountryId);
                    continue;
                }

                var filteredZoneChange = new ZoneChanges
                {
                    ZoneId = zoneChange.ZoneId,
                    ZoneName = zoneChange.ZoneName,
                    CountryId = zoneChange.CountryId,
                    NewOtherRateBED = zoneChange.NewOtherRateBED
                };

                if (zoneChange.NewRates != null && zoneChange.NewRates.Any())
                    filteredZoneChange.NewRates = FilterZoneNewRates(customerId, sellingProductId, zoneChange.NewRates, customerCountry.BED, effectiveDate, pricingSettings.IncreasedRateDayOffset.Value, pricingSettings.DecreasedRateDayOffset.Value, effectiveRateLocator, cuurentRateLocator, currencyId, zoneChange.ZoneId, followPublisherRatesBED, excludedCountries, notSoldCountryIds, countriesWithDifferentCurrencies);

                if (zoneChange.ClosedRates != null && zoneChange.ClosedRates.Any())
                    filteredZoneChange.ClosedRates = FilterZoneClosedRates(customerId, zoneChange.ClosedRates, customerCountry.BED, closedRateLocator, sellingProductId, currencyId, excludedCountries, notSoldCountryIds);

                if (zoneChange.NewRoutingProduct != null && customerCountry.BED <= zoneChange.NewRoutingProduct.BED)
                {
                    filteredZoneChange.NewRoutingProduct = zoneChange.NewRoutingProduct;
                }
                else if (zoneChange.RoutingProductChange != null)
                {
                    var currentRoutingProduct = zoneRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneChange.ZoneId);

                    if (currentRoutingProduct != null && currentRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone)
                        filteredZoneChange.RoutingProductChange = zoneChange.RoutingProductChange;
                }
                else if (followPublisherRoutingProduct && filteredZoneChange.NewRates != null && filteredZoneChange.NewRates.Any(item => item.RateTypeId == null))
                {

                    var subscriberRoutingProduct = zoneNewRateRoutingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneChange.ZoneId);
                    var publisherRoutingProduct = zoneNewRateRoutingProductLocator.GetCustomerZoneRoutingProduct(publisherId, publisherSellingProductId, zoneChange.ZoneId);

                    if (subscriberRoutingProduct.RoutingProductId != publisherRoutingProduct.RoutingProductId)
                    {
                        var draftNewSaleZoneRoutingProduct = new DraftNewSaleZoneRoutingProduct()
                        {
                            ZoneId = zoneChange.ZoneId,
                            ZoneRoutingProductId = publisherRoutingProduct.RoutingProductId,
                            BED = publisherRoutingProduct.BED,
                            EED = publisherRoutingProduct.EED,
                        };
                        filteredZoneChange.NewRoutingProduct = draftNewSaleZoneRoutingProduct;
                    }
                }

                filteredZoneChanges.Add(filteredZoneChange);
            }
            if (notSoldCountryIds.Count() > 0)
            {
                var notSoldCountryNames = countryManager.GetCountryNames(notSoldCountryIds);
                excludedCountries.Add(new ExcludedChange()
                    {
                        SubscriberId = customerId,
                        CountryIds = notSoldCountryIds,
                        Reason = String.Format("Country(ies) : '{0}' not sold to subscriber '{1}'", String.Join(" , ", notSoldCountryNames), carrierAccountManager.GetCarrierAccountName(customerId))
                    });
            }
            if (countriesWithDifferentCurrencies.Count() > 0)
            {
                var countriesWithDifferentCurrenciesNames = countryManager.GetCountryNames(countriesWithDifferentCurrencies);
                excludedCountries.Add(new ExcludedChange()
                {
                    SubscriberId = customerId,
                    CountryIds = countriesWithDifferentCurrencies,
                    Reason = String.Format("Country(ies) : '{0}' have different currencies than publisher", String.Join(" , ", countriesWithDifferentCurrenciesNames))
                });
            }
            return filteredZoneChanges;
        }

        private IEnumerable<DraftRateToChange> FilterZoneNewRates(int customerId, int sellingProductId, IEnumerable<DraftRateToChange> newRates, DateTime countrySellDate, DateTime effectiveDate, int increasedRateDayOffset, int decreasedRateDayOffset, SaleEntityZoneRateLocator effectiveRateLocator, SaleEntityZoneRateLocator cuurentRateLocator, int currencyId, long zoneId, bool followPublisherRatesBED, List<ExcludedChange> excludedCountries, List<int> notSoldCountryIds, List<int> countriesWithDifferentCurrencies)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierName = carrierAccountManager.GetCarrierAccountName(customerId);
            CountryManager countryManager = new CountryManager();
            var filteredNewRates = new List<DraftRateToChange>();
            var rateAtActionDate = effectiveRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);
            var currentRate = new SaleEntityZoneRate();
            if (!followPublisherRatesBED)
                currentRate = cuurentRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId);

            foreach (var newRate in newRates)
            {
                if (newRate.BED < countrySellDate)
                {
                    var notSoldCountryNames = countryManager.GetCountryNames(notSoldCountryIds);
                    excludedCountries.Add(new ExcludedChange()
                    {
                        SubscriberId = customerId,
                        CountryIds = notSoldCountryIds,
                        Reason = String.Format("Country(ies) : '{0}' not sold to subscriber '{1}'", String.Join(" , ", notSoldCountryNames), carrierName)
                    });
                    continue;
                }
                if (rateAtActionDate == null)
                {
                    var notSoldCountryNames = countryManager.GetCountryNames(notSoldCountryIds);
                    excludedCountries.Add(new ExcludedChange()
                    {
                        SubscriberId = customerId,
                        CountryIds = notSoldCountryIds,
                        Reason = String.Format("Country(ies) : '{0}' not sold to subscriber '{1}'", String.Join(" , ", notSoldCountryNames), carrierName)
                    });
                    continue;
                }

                if (rateAtActionDate != null && rateAtActionDate.EffectiveCurrencyId != currencyId)
                {
                    SaleZoneManager saleZoneManager = new SaleZoneManager();
                    var countryId = saleZoneManager.GetSaleZoneCountryId(newRate.ZoneId);
                    countriesWithDifferentCurrencies.Add(countryId.Value);
                    continue;
                }

                if (!followPublisherRatesBED)
                {
                    SaleRate matchedRate;
                    if (!newRate.RateTypeId.HasValue)
                    {
                        matchedRate = currentRate.Rate;
                    }
                    else currentRate.RatesByRateType.TryGetValue(newRate.RateTypeId.Value, out matchedRate);

                    if (matchedRate != null)
                    {
                        if (newRate.Rate <= matchedRate.Rate)
                            newRate.BED = countrySellDate > effectiveDate.AddDays(decreasedRateDayOffset) ? countrySellDate : effectiveDate.AddDays(decreasedRateDayOffset);
                        else newRate.BED = countrySellDate > effectiveDate.AddDays(increasedRateDayOffset) ? countrySellDate : effectiveDate.AddDays(increasedRateDayOffset);
                    }
                    else
                    {
                        newRate.BED = countrySellDate > effectiveDate ? countrySellDate : effectiveDate;
                    }
                }
                filteredNewRates.Add(newRate);
            }
            return filteredNewRates;
        }

        private IEnumerable<DraftRateToClose> FilterZoneClosedRates(int customerId, IEnumerable<DraftRateToClose> closedRates, DateTime countrySellDate, SaleEntityZoneRateLocator rateLocator, int sellingProductId, int currencyId, List<ExcludedChange> excludedCountries, List<int> notSoldCountryIds)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierName = carrierAccountManager.GetCarrierAccountName(customerId);
            CountryManager countryManager = new CountryManager();
            var filteredClosedRates = new List<DraftRateToClose>();
            foreach (var closedRate in closedRates)
            {
                if (closedRate.EED < countrySellDate)
                {
                    var notSoldCountryNames = countryManager.GetCountryNames(notSoldCountryIds);
                    excludedCountries.Add(new ExcludedChange()
                    {
                        SubscriberId = customerId,
                        CountryIds = notSoldCountryIds,
                        Reason = String.Format("Country(ies) : '{0}' not sold to subscriber '{1}'", String.Join(" , ", notSoldCountryNames), carrierName)
                    });
                    continue;
                }
                var currentRate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId, closedRate.ZoneId);
                if (currentRate != null && currentRate.Source == SalePriceListOwnerType.Customer && currentRate.EffectiveCurrencyId == currencyId && (closedRate.RateTypeId == null || currentRate.RatesByRateType.ContainsKey(closedRate.RateTypeId.Value)))
                    filteredClosedRates.Add(closedRate);
            }
            return filteredClosedRates;
        }

        private Dictionary<int, CustomerCountry2> GetNotEndedCustomerCountriesByCountryId(int customerId, DateTime effectiveDate)
        {
            var customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetNotEndedCustomerCountriesEffectiveAfter(customerId, effectiveDate);
            var customerCountriesSoldDateByCountryId = new Dictionary<int, CustomerCountry2>();

            if (customerCountries != null)
            {
                foreach (var customerCountry in customerCountries)
                    customerCountriesSoldDateByCountryId.GetOrCreateItem(customerCountry.CountryId, () =>
                    {
                        return customerCountry;
                    });
            }
            return customerCountriesSoldDateByCountryId;
        }

        private SaleEntityZoneRateLocator GetClosedRateLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges, int sellingProductId)
        {
            var zonesWithClosedRates = zoneChanges.FindAllRecords(item => item.ClosedRates != null && item.ClosedRates.Any());
            if (!zonesWithClosedRates.Any())
                return null;

            DateTime minimumDate = zonesWithClosedRates.Min(item => item.ClosedRates.Min(cr => cr.EED));
            IEnumerable<long> zoneIds = zonesWithClosedRates.Select(item => item.ZoneId);
            Dictionary<long, DateTime> rateActionDateByZoneId = new Dictionary<long, DateTime>();

            foreach (var zoneChange in zonesWithClosedRates)
            {
                rateActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.ClosedRates.Min(item => item.EED);
                });
            }
            return (new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(customerId, sellingProductId, zoneIds, minimumDate, rateActionDateByZoneId)));
        }
        private SaleEntityZoneRateLocator GetNewRateLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges, int sellingProductId)
        {
            var zonesWithNewRates = zoneChanges.FindAllRecords(item => item.NewRates != null && item.NewRates.Any());
            if (!zonesWithNewRates.Any())
                return null;

            DateTime minimumDate = zonesWithNewRates.Min(item => item.NewRates.Min(cr => cr.BED));
            IEnumerable<long> zoneIds = zonesWithNewRates.Select(item => item.ZoneId);
            Dictionary<long, DateTime> rateActionDateByZoneId = new Dictionary<long, DateTime>();

            foreach (var zoneChange in zonesWithNewRates)
            {
                rateActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.NewRates.Min(item => item.BED);
                });
            }
            return (new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(customerId, sellingProductId, zoneIds, minimumDate, rateActionDateByZoneId)));
        }
        private SaleEntityZoneRoutingProductLocator GetZoneRoutingProductLocator(int customerId, IEnumerable<ZoneChanges> zoneChanges)
        {
            var zoneIdsWithActionDate = new Dictionary<long, DateTime>();

            var zonesWithRoutingProductChange = zoneChanges.FindAllRecords(item => item.RoutingProductChange != null);
            if (!zonesWithRoutingProductChange.Any())
                return null;
            foreach (var zoneChange in zonesWithRoutingProductChange)
            {
                zoneIdsWithActionDate.GetOrCreateItem(zoneChange.ZoneId, () =>
                    {
                        return zoneChange.RoutingProductChange.EED;
                    });
            }
            return (new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int>() { customerId }, zoneIdsWithActionDate)));
        }

        private SaleEntityZoneRoutingProductLocator GetZoneNewRateRoutingProductLocator(int customerId, int publisherId, IEnumerable<ZoneChanges> zoneChanges)
        {
            var zoneIdsWithActionDate = new Dictionary<long, DateTime>();

            var zonesWithNewNormalRateChange = zoneChanges.FindAllRecords(item => item.NewRates != null && item.NewRates.Any(x => x.RateTypeId == null));
            if (!zonesWithNewNormalRateChange.Any())
                return null;
            foreach (var zoneChange in zonesWithNewNormalRateChange)
            {
                zoneIdsWithActionDate.GetOrCreateItem(zoneChange.ZoneId, () =>
                {
                    return zoneChange.NewRates.FindRecord(x => x.RateTypeId == null).BED;
                });
            }
            return (new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadByRateBED(new List<int>() { customerId, publisherId }, zoneIdsWithActionDate)));
        }

        /* private SaleEntityZoneRateLocator GetRateLocator(int customerId, IEnumerable<ZoneChanges> zonesWithNewRates, IEnumerable<ZoneChanges> zonesWithClosedRates, int sellingProductId)
         {
             DateTime? minimumNewRatesDate = null; 
             DateTime? minimumClosedRatesDate = null;

             List<long> zoneIds = new List<long>();
             Dictionary<long, DateTime> zoneActionDateByZoneId = new Dictionary<long, DateTime>();

             if (zonesWithNewRates != null)
             {
                 minimumNewRatesDate = zonesWithNewRates.Min(item => item.NewRates.Min(cr => cr.BED));
                 zoneIds.AddRange(zonesWithNewRates.Select(item => item.ZoneId));

                 foreach (var zoneChange in zonesWithNewRates)
                 {
                     zoneActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                     {
                         return zoneChange.NewRates.Min(item => item.BED);
                     });
                 }
             }

             if (zonesWithClosedRates != null)
             {
                 minimumClosedRatesDate = zonesWithClosedRates.Min(item => item.ClosedRates.Min(cr => cr.EED));
                 zoneIds.AddRange(zonesWithClosedRates.Select(item => item.ZoneId));

                 foreach (var zoneChange in zonesWithClosedRates)
                 {
                     zoneActionDateByZoneId.GetOrCreateItem(zoneChange.ZoneId, () =>
                                         {
                                             return zoneChange.ClosedRates.Min(item => item.EED);
                                         });
                 }
             }

             DateTime minimumDate = TOne.WhS.Sales.Business.UtilitiesManager.GetMinDate(new List<DateTime?> { minimumNewRatesDate, minimumClosedRatesDate }).Value;
             return (new SaleEntityZoneRateLocator(new SaleRateReadRPChanges(customerId, sellingProductId, zoneIds, minimumDate, zoneActionDateByZoneId)));
         }
    */

    }
}
