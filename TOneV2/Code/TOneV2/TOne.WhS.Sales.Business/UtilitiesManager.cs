using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class UtilitiesManager
    {
        #region Public Methods

        public static DateTime? GetMaxDate(IEnumerable<DateTime?> dates)
        {
            int count;
            DateTime? maxDate = GetFirstDate(dates, out count);

            if (count == 1)
                return maxDate;

            for (int i = 1; i < count; i++)
            {
                DateTime? date = dates.ElementAt(i);
                if (date.HasValue)
                {
                    if (!maxDate.HasValue)
                        maxDate = date;
                    else if (date.Value > maxDate.Value)
                        maxDate = date;
                }
            }

            return maxDate;
        }

        public static DateTime? GetMinDate(IEnumerable<DateTime?> dates)
        {
            int count;
            DateTime? minDate = GetFirstDate(dates, out count);

            if (count == 1)
                return minDate;

            for (int i = 1; i < count; i++)
            {
                DateTime? date = dates.ElementAt(i);
                if (date.HasValue)
                {
                    if (!minDate.HasValue)
                        minDate = date;
                    else if (date.Value < minDate.Value)
                        minDate = date;
                }
            }

            return minDate;
        }

        public static string GetDateTimeAsString(DateTime dateTime)
        {
            return dateTime.ToShortDateString();
        }

        public static bool IsActionApplicableToCountry(IBulkActionApplicableToCountryContext context, Func<IActionApplicableToZoneContext, bool> isActionApplicableToZone)
        {
            var bulkActionApplicableToAnyCountryZoneInput = new BulkActionApplicableToAnyCountryZoneInput()
            {
                CountryId = context.Country.CountryId,
                OwnerSellingNumberPlanId = context.OwnerSellingNumberPlanId,
                OwnerType = context.OwnerType,
                OwnerId = context.OwnerId,
                ZoneDraftsByZoneId = context.ZoneDraftsByZoneId,
                CountryBEDsByCountryId = context.CountryBEDsByCountryId,
                CountryEEDsByCountryId = context.CountryEEDsByCountryId,
                GetSellingProductZoneRate = context.GetSellingProductZoneRate,
                GetCustomerZoneRate = context.GetCustomerZoneRate,
                GetCurrentSellingProductZoneRP = context.GetCurrentSellingProductZoneRP,
                GetCurrentCustomerZoneRP = context.GetCurrentCustomerZoneRP,
                GetRateBED = context.GetRateBED,
                IsBulkActionApplicableToZone = isActionApplicableToZone
            };
            return IsBulkActionApplicableToAnyCountryZone(bulkActionApplicableToAnyCountryZoneInput);
        }

        public static bool IsActionApplicableToZone(BulkActionApplicableToZoneInput input)
        {
            ZoneChanges zoneDraft = null;

            if (input.Draft != null && input.Draft.ZoneChanges != null)
                zoneDraft = input.Draft.ZoneChanges.FindRecord(x => x.ZoneId == input.SaleZone.SaleZoneId);

            var actionApplicableToZoneContext = new ActionApplicableToZoneContext(input.GetCurrentSellingProductZoneRP, input.GetCurrentCustomerZoneRP, input.GetSellingProductZoneRate, input.GetCustomerZoneRate, input.GetRateBED, input.CountryBEDsByCountryId, input.CountryEEDsByCountryId)
            {
                OwnerType = input.OwnerType,
                OwnerId = input.OwnerId,
                SaleZone = input.SaleZone,
                ZoneDraft = zoneDraft
            };

            return input.BulkAction.IsApplicableToZone(actionApplicableToZoneContext);
        }

        public static Dictionary<int, DateTime> GetDatesByCountry(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            Dictionary<int, DateTime> datesByCountry = new CustomerCountryManager().GetSoldCountryDatesByCountryId(customerId, effectiveOn, isEffectiveInFuture);

            if (datesByCountry == null)
                datesByCountry = new Dictionary<int, DateTime>();

            Changes draft = new RatePlanDraftManager().GetDraft(SalePriceListOwnerType.Customer, customerId);
            if (draft != null && draft.CountryChanges != null && draft.CountryChanges.NewCountries != null)
            {
                foreach (DraftNewCountry newCountry in draft.CountryChanges.NewCountries)
                {
                    if (!datesByCountry.ContainsKey(newCountry.CountryId))
                        datesByCountry.Add(newCountry.CountryId, newCountry.BED);
                }
            }
            return datesByCountry;
        }

        public static bool IsCustomerZoneCountryApplicable(int countryId, DateTime rateBED, Dictionary<int, DateTime> datesByCountry)
        {
            DateTime soldOn;

            if (!datesByCountry.TryGetValue(countryId, out soldOn))
                return false;

            if (soldOn > rateBED)
                return false;

            return true;
        }

        public static bool CustomerZoneHasPendingClosedNormalRate(int customerId, int sellingProductId, long zoneId, Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate)
        {
            SaleEntityZoneRate customerZoneRate = getCustomerZoneRate(customerId, sellingProductId, zoneId, false);
            return (customerZoneRate != null && customerZoneRate.Rate != null && customerZoneRate.Rate.EED.HasValue);
        }

        public static int? GetCostCalculationMethodIndex(IEnumerable<CostCalculationMethod> costCalculationMethods, Guid costCalculationMethodConfigId)
        {
            if (costCalculationMethods != null)
            {
                for (int i = 0; i < costCalculationMethods.Count(); i++)
                {
                    if (costCalculationMethods.ElementAt(i).ConfigId.Equals(costCalculationMethodConfigId))
                        return i;
                }
            }
            return null;
        }

        public static bool IsBulkActionApplicableToAnyCountryZone(BulkActionApplicableToAnyCountryZoneInput input)
        {
            var saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone> saleZones = saleZoneManager.GetSaleZonesByCountryId(input.OwnerSellingNumberPlanId, input.CountryId, DateTime.Today);

            if (saleZones == null || saleZones.Count() == 0)
                return false;

            foreach (SaleZone saleZone in saleZones)
            {
                var bulkActionApplicableToZoneContext = new ActionApplicableToZoneContext(input.GetCurrentSellingProductZoneRP, input.GetCurrentCustomerZoneRP, input.GetSellingProductZoneRate, input.GetCustomerZoneRate, input.GetRateBED, input.CountryBEDsByCountryId, input.CountryEEDsByCountryId)
                {
                    OwnerType = input.OwnerType,
                    OwnerId = input.OwnerId,
                    SaleZone = saleZone,
                    ZoneDraft = input.ZoneDraftsByZoneId.GetRecord(saleZone.SaleZoneId)
                };

                if (input.IsBulkActionApplicableToZone(bulkActionApplicableToZoneContext))
                    return true;
            }

            return false;
        }

        public static void SetDraftRateBEDs(SalePriceListOwnerType ownerType, int ownerId, out DateTime newRateBED, out DateTime increasedRateBED, out DateTime decreasedRateBED)
        {
            var pricingSettings = GetPricingSettings(ownerType, ownerId);

            newRateBED = DateTime.Today.AddDays(pricingSettings.NewRateDayOffset.Value);
            increasedRateBED = DateTime.Today.AddDays(pricingSettings.IncreasedRateDayOffset.Value);
            decreasedRateBED = DateTime.Today.AddDays(pricingSettings.DecreasedRateDayOffset.Value);
        }

        public static PricingSettings GetPricingSettings(SalePriceListOwnerType ownerType, int ownerId)
        {
            var pricingSettings = new PricingSettings();

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                var sellingProductManager = new TOne.WhS.BusinessEntity.Business.SellingProductManager();
                pricingSettings = sellingProductManager.GetSellingProductPricingSettings(ownerId);
            }

            else
            {
                var carrierAccountManager = new TOne.WhS.BusinessEntity.Business.CarrierAccountManager();
                pricingSettings = carrierAccountManager.GetCustomerPricingSettings(ownerId);
            }
            return pricingSettings;
        }

        public static DateTime GetDraftRateBED(decimal? currentRateValue, decimal newRateValue, DateTime newRateBED, DateTime increasedRateBED, DateTime decreasedRateBED)
        {
            if (!currentRateValue.HasValue)
                return newRateBED;
            else if (currentRateValue.Value > newRateValue)
                return increasedRateBED;
            else if (currentRateValue.Value < newRateValue)
                return decreasedRateBED;
            else
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The current Rate '{0}' is the same as the new Rate", currentRateValue.Value));
        }

        public static IEnumerable<int> GetClosedCountryIds(int customerId, Changes draft, DateTime effectiveOn)
        {
            var closedCountryIds = new List<int>();

            if (draft != null && draft.CountryChanges != null && draft.CountryChanges.ChangedCountries != null && draft.CountryChanges.ChangedCountries.Countries != null)
                closedCountryIds.AddRange(draft.CountryChanges.ChangedCountries.Countries.MapRecords(x => x.CountryId));

            IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetSoldCountries(customerId, effectiveOn);
            if (soldCountries != null)
            {
                IEnumerable<int> closedSoldCountryIds = soldCountries.MapRecords(x => x.CountryId, x => x.EED.HasValue);
                if (closedSoldCountryIds != null)
                    closedCountryIds.AddRange(closedSoldCountryIds);
            }

            return closedCountryIds;
        }

        public static Dictionary<long, DateTime> GetZoneEffectiveDatesByZoneId(IEnumerable<SaleZone> saleZones, Dictionary<int, DateTime> countryBEDsByCountryId)
        {
            var zoneEffectiveDatesByZoneId = new Dictionary<long, DateTime>();
            DateTime today = DateTime.Today;
            foreach (SaleZone saleZone in saleZones)
            {
                DateTime countryBED;
                if (!countryBEDsByCountryId.TryGetValue(saleZone.CountryId, out countryBED))
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The BED of country '{0}' of zone '{1}' was not found", saleZone.CountryId, saleZone.Name));
                if (!zoneEffectiveDatesByZoneId.ContainsKey(saleZone.SaleZoneId))
                {
                    DateTime zoneEffectiveDate = Utilities.Max(Utilities.Max(today, countryBED), saleZone.BED);
                    zoneEffectiveDatesByZoneId.Add(saleZone.SaleZoneId, zoneEffectiveDate);
                }
            }
            return zoneEffectiveDatesByZoneId;
        }
        public static Dictionary<long, DateTime> GetZoneEffectiveDatesByZoneId(IEnumerable<SaleZone> saleZones)
        {
            var zoneEffectiveDatesByZoneId = new Dictionary<long, DateTime>();
            DateTime today = DateTime.Today;
            foreach (SaleZone saleZone in saleZones)
            {
                if (!zoneEffectiveDatesByZoneId.ContainsKey(saleZone.SaleZoneId))
                {
                    DateTime zoneEffectiveDate = Utilities.Max(today, saleZone.BED);
                    zoneEffectiveDatesByZoneId.Add(saleZone.SaleZoneId, zoneEffectiveDate);
                }
            }
            return zoneEffectiveDatesByZoneId;
        }
        public static void SetBulkActionContextCurrentRateHelpers(ISaleRateReader rateReader, out SaleEntityZoneRateLocator rateLocator, out Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate, out Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate)
        {
            var rateLocatorValue = new SaleEntityZoneRateLocator(rateReader);
            rateLocator = rateLocatorValue;
            getSellingProductZoneRate = (sellingProductId, zoneId) => { return rateLocatorValue.GetSellingProductZoneRate(sellingProductId, zoneId); };
            getCustomerZoneRate = (customerId, sellingProductId, zoneId) => { return rateLocatorValue.GetCustomerZoneRate(customerId, sellingProductId, zoneId); };
        }

        public static void SetBulkActionContextHelpers(SalePriceListOwnerType ownerType, int ownerId, Func<int, long, SaleEntityZoneRate> getSellingProductZoneCurrentRate, Func<int, int, long, SaleEntityZoneRate> getCustomerZoneCurrentRate, out Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate, out Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate, out Func<int, long, SaleEntityZoneRoutingProduct> getSellingProductZoneCurrentRP, out Func<int, int, long, SaleEntityZoneRoutingProduct> getCustomerZoneCurrentRP, out Dictionary<int, DateTime> countryBEDsByCountryId, out Dictionary<int, DateTime> countryEEDsByCountryId)
        {
            var futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
            var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Today));

            getSellingProductZoneRate = (sellingProductId, zoneId, shouldGetFutureRate) =>
            {
                return (shouldGetFutureRate) ? futureRateLocator.GetSellingProductZoneRate(sellingProductId, zoneId) : getSellingProductZoneCurrentRate(sellingProductId, zoneId);
            };

            getCustomerZoneRate = (customerId, sellingProductId, zoneId, shouldGetFutureRate) =>
            {
                return (shouldGetFutureRate) ? futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, zoneId) : getCustomerZoneCurrentRate(customerId, sellingProductId, zoneId);
            };

            getSellingProductZoneCurrentRP = (sellingProductId, zoneId) =>
            {
                return routingProductLocator.GetSellingProductZoneRoutingProduct(sellingProductId, zoneId);
            };

            getCustomerZoneCurrentRP = (customerId, sellingProductId, zoneId) =>
            {
                return routingProductLocator.GetCustomerZoneRoutingProduct(customerId, sellingProductId, zoneId);
            };

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                countryBEDsByCountryId = GetDatesByCountry(ownerId, DateTime.Today, true);
                Changes draft = new RatePlanDraftManager().GetDraft(SalePriceListOwnerType.Customer, ownerId);
                DraftChangedCountries draftChangedCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.ChangedCountries : null;
                countryEEDsByCountryId = GetCountryEEDsByCountryId(ownerId, draftChangedCountries, DateTime.Today);
            }
            else
            {
                countryBEDsByCountryId = null;
                countryEEDsByCountryId = null;
            }
        }

        public static decimal ConvertToCurrencyAndRound(decimal value, int fromCurrencyId, int toCurrencyId, DateTime exchangeRateDate, int decimalPrecision, Vanrise.Common.Business.CurrencyExchangeRateManager exchangeRateManager)
        {
            if (fromCurrencyId == toCurrencyId)
                return value;
            decimal convertedValue = exchangeRateManager.ConvertValueToCurrency(value, fromCurrencyId, toCurrencyId, exchangeRateDate);
            return decimal.Round(convertedValue, decimalPrecision);
        }

        public static bool RateSourcesContain(SaleEntityZoneRate rate, IEnumerable<RateSource> rateSources)
        {
            if (rate.Source == SalePriceListOwnerType.SellingProduct && !rateSources.Contains(RateSource.Inherited))
                return false;

            if (rate.Source == SalePriceListOwnerType.Customer && !rateSources.Contains(RateSource.Explicit))
                return false;

            return true;
        }

        public static Dictionary<int, DateTime> GetCountryEEDsByCountryId(int customerId, DraftChangedCountries draftChangedCountries, DateTime effectiveAfter)
        {
            var countryEEDsByCountryId = new Dictionary<int, DateTime>();

            if (draftChangedCountries != null && draftChangedCountries.Countries != null)
            {
                foreach (DraftChangedCountry draftChangedCountry in draftChangedCountries.Countries)
                {
                    if (!countryEEDsByCountryId.ContainsKey(draftChangedCountry.CountryId))
                        countryEEDsByCountryId.Add(draftChangedCountry.CountryId, draftChangedCountries.EED);
                }
            }

            IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetEndedCustomerCountriesEffectiveAfter(customerId, effectiveAfter);

            if (soldCountries != null && soldCountries.Count() >= 0)
            {
                foreach (CustomerCountry2 soldCountry in soldCountries)
                {
                    if (!countryEEDsByCountryId.ContainsKey(soldCountry.CountryId))
                        countryEEDsByCountryId.Add(soldCountry.CountryId, soldCountry.EED.Value);
                }
            }

            return countryEEDsByCountryId;
        }

        #endregion

        #region Private Methods
        private static DateTime? GetFirstDate(IEnumerable<DateTime?> dates, out int count)
        {
            if (dates == null)
                throw new ArgumentNullException("dates");

            count = dates.Count();
            if (count == 0)
                throw new ArgumentException("dates.Count = 0");

            return dates.ElementAt(0);
        }
        #endregion
    }

    #region Public Classes

    public class BulkActionApplicableToAnyCountryZoneInput
    {
        public int CountryId { get; set; }

        public int OwnerSellingNumberPlanId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

        public Dictionary<int, DateTime> CountryBEDsByCountryId { get; set; }

        public Dictionary<int, DateTime> CountryEEDsByCountryId { get; set; }

        public Func<int, long, SaleEntityZoneRoutingProduct> GetCurrentSellingProductZoneRP { get; set; }

        public Func<int, int, long, SaleEntityZoneRoutingProduct> GetCurrentCustomerZoneRP { get; set; }

        public Func<int, long, bool, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

        public Func<int, int, long, bool, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }

        public Func<decimal?, decimal, DateTime> GetRateBED { get; set; }

        public Func<IActionApplicableToZoneContext, bool> IsBulkActionApplicableToZone { get; set; }
    }

    public class BulkActionApplicableToZoneInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public SaleZone SaleZone { get; set; }

        public BulkActionType BulkAction { get; set; }

        public Changes Draft { get; set; }

        public Dictionary<int, DateTime> CountryBEDsByCountryId { get; set; }

        public Dictionary<int, DateTime> CountryEEDsByCountryId { get; set; }

        public Func<int, long, SaleEntityZoneRoutingProduct> GetCurrentSellingProductZoneRP { get; set; }

        public Func<int, int, long, SaleEntityZoneRoutingProduct> GetCurrentCustomerZoneRP { get; set; }

        public Func<int, long, bool, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

        public Func<int, int, long, bool, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }

        public Func<decimal?, decimal, DateTime> GetRateBED { get; set; }
    }

    #endregion
}
