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

        public static bool IsActionApplicableToZone(IsActionApplicableToZoneInput input)
        {
            ZoneChanges zoneDraft = null;

            if (input.Draft != null && input.Draft.ZoneChanges != null)
                zoneDraft = input.Draft.ZoneChanges.FindRecord(x => x.ZoneId == input.SaleZone.SaleZoneId);

            var actionApplicableToZoneContext = new ActionApplicableToZoneContext(input.GetSellingProductZoneRate, input.GetCustomerZoneRate, input.GetRateBED)
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
            var datesByCountry = new Dictionary<int, DateTime>();

            IEnumerable<CustomerCountry2> customerCountries = new CustomerCountryManager().GetCustomerCountries(customerId, effectiveOn, false);
            if (customerCountries != null)
            {
                foreach (CustomerCountry2 customerCountry in customerCountries)
                    if (!datesByCountry.ContainsKey(customerCountry.CountryId))
                        datesByCountry.Add(customerCountry.CountryId, customerCountry.BED);
            }

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
                var bulkActionApplicableToZoneContext = new ActionApplicableToZoneContext(input.GetSellingProductZoneRate, input.GetCustomerZoneRate, input.GetRateBED)
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

        public static void SetDraftRateBEDs(out DateTime newRateBED, out DateTime increasedRateBED, out DateTime decreasedRateBED)
        {
            var configManager = new ConfigManager();
            newRateBED = DateTime.Today.AddDays(configManager.GetNewRateDayOffset());
            increasedRateBED = DateTime.Today.AddDays(configManager.GetIncreasedRateDayOffset());
            decreasedRateBED = DateTime.Today.AddDays(configManager.GetDecreasedRateDayOffset());
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

    public class BulkActionApplicableToAnyCountryZoneInput
    {
        public int CountryId { get; set; }

        public int OwnerSellingNumberPlanId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

        public Func<int, long, bool, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

        public Func<int, int, long, bool, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }

        public Func<decimal?, decimal, DateTime> GetRateBED { get; set; }

        public Func<IActionApplicableToZoneContext, bool> IsBulkActionApplicableToZone { get; set; }
    }

    public class IsActionApplicableToZoneInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public SaleZone SaleZone { get; set; }

        public BulkActionType BulkAction { get; set; }

        public Changes Draft { get; set; }

        public Func<int, long, bool, SaleEntityZoneRate> GetSellingProductZoneRate { get; set; }

        public Func<int, int, long, bool, SaleEntityZoneRate> GetCustomerZoneRate { get; set; }

        public Func<decimal?, decimal, DateTime> GetRateBED { get; set; }
    }
}
