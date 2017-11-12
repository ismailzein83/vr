using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class BusinessRuleUtilities
    {
        public static IEnumerable<string> GetZoneActionMessages(DataByZone zone)
        {
            var actionMessages = new List<string>();
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

            if (zone.NormalRateToChange != null)
                actionMessages.Add("A new normal rate is defined");

            if (zone.NormalRateToClose != null)
                actionMessages.Add("An existing normal rate is closed");

            if (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Count > 0)
            {
                var rateTypeNames = new List<string>();
                foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
                    AddRateTypeName(rateTypeNames, otherRateToChange.RateTypeId.Value, rateTypeManager);
                actionMessages.Add(string.Format("Other rates '{0}' are defined", string.Join(",", rateTypeNames)));
            }

            if (zone.OtherRatesToClose != null && zone.OtherRatesToClose.Count > 0)
            {
                var rateTypeNames = new List<string>();
                foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
                    AddRateTypeName(rateTypeNames, otherRateToClose.RateTypeId.Value, rateTypeManager);
                actionMessages.Add(string.Format("Existing other rates '{0}' are closed", string.Join(",", rateTypeNames)));
            }

            if (zone.SaleZoneRoutingProductToAdd != null)
                actionMessages.Add("A new routing product is defined");

            if (zone.SaleZoneRoutingProductToClose != null)
                actionMessages.Add("An existing routing product is closed");

            return actionMessages;
        }
        public static bool IsAnyZoneRateGreaterThanMaxRate(DataByZone zoneData, IRatePlanContext ratePlanContext)
        {
            if (zoneData.NormalRateToChange != null)
            {
                if (IsRateGreaterThanMaxRate(zoneData.NormalRateToChange, ratePlanContext))
                    return true;
            }
            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (IsRateGreaterThanMaxRate(otherRateToChange, ratePlanContext))
                        return true;
                }
            }
            return false;
        }
        public static bool IsAnyZoneRateNegative(DataByZone zoneData)
        {
            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.NormalRate <= 0)
                return true;
            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Any(x => x.NormalRate <= 0))
                return true;
            return false;
        }
        public static bool IsAnyZoneNewRateEnded(DataByZone zoneData)
        {
            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
                return true;
            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.EED.HasValue)
                        return true;
                }
            }
            return false;
        }

        #region DoZoneRateCurrenciesConflict
        public static bool DoZoneRateCurrenciesConflict(CountryData countryData, IRatePlanContext ratePlanContext, out string errorMessage)
        {
            Dictionary<long, RateToChange> newRatesByZoneId;
            Dictionary<long, RateToClose> closedRatesByZoneId;
            DateTime? minRateActionDate;

            if (!DoRateActionsExist(countryData.ZoneDataByZoneId, out newRatesByZoneId, out closedRatesByZoneId, out minRateActionDate))
            {
                errorMessage = null;
                return false;
            }

            string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);

            IEnumerable<ExistingZone> existingZones = ratePlanContext.ExistingZonesByCountry.GetRecord(countryData.CountryId);
            IEnumerable<ExistingZone> effectiveOrFutureZones = GetEffectiveOrFutureZones(existingZones, countryName, countryData.CountryBED);

            if (closedRatesByZoneId.Count > 0 && closedRatesByZoneId.Count < effectiveOrFutureZones.Count() && ratePlanContext.CurrencyId != ratePlanContext.SellingProductCurrencyId)
            {
                var currencyManager = new Vanrise.Common.Business.CurrencyManager();
                string processCurrencySymbol = currencyManager.GetCurrencySymbol(ratePlanContext.CurrencyId);
                string sellingProductCurrencySymbol = currencyManager.GetCurrencySymbol(ratePlanContext.SellingProductCurrencyId);
                errorMessage = string.Format("When pricing with a currency '{0}' different than selling product currency '{1}', rates of zones for country '{2}' must be all inherited or explicit", processCurrencySymbol, sellingProductCurrencySymbol, countryName);
                return true;
            }

            foreach (ExistingZone countryZone in effectiveOrFutureZones)
            {
                RateToChange newRate = newRatesByZoneId.GetRecord(countryZone.ZoneId);
                RateToClose closedRate = closedRatesByZoneId.GetRecord(countryZone.ZoneId);

                DateTime? zoneRateActionDate = null;

                if (newRate != null)
                {
                    if (newRate.BED == minRateActionDate.Value)
                        continue;
                    zoneRateActionDate = newRate.BED;
                }
                else if (closedRate != null)
                {
                    if (closedRate.CloseEffectiveDate == minRateActionDate.Value)
                        continue;
                    zoneRateActionDate = closedRate.CloseEffectiveDate;
                }

                DateTime? xDate = Max(countryZone.BED, countryData.CountryBED, minRateActionDate.Value);
                DateTime? yDate = Min(countryZone.EED, countryData.CountryEED, zoneRateActionDate);

                if (!yDate.HasValue || yDate.Value > xDate)
                {
                    IEnumerable<ExistingRate> effectiveOrFutureRates = GetEffectiveOrFutureRates(countryZone.ExistingRates, countryData.CountryBED);

                    if (AreZoneRatesInConflict(effectiveOrFutureRates, xDate.Value, yDate, ratePlanContext))
                    {
                        errorMessage = string.Format("When changing currency, rates of zones for country '{0}' must be all with the same date", countryName);
                        return true;
                    }
                }
            }

            errorMessage = null;
            return false;
        }
        private static bool DoRateActionsExist(Dictionary<long, DataByZone> dataByZoneId, out Dictionary<long, RateToChange> newRatesByZoneId, out Dictionary<long, RateToClose> closedRatesByZoneId, out DateTime? minRateActionDate)
        {
            if (dataByZoneId == null || dataByZoneId.Count == 0)
            {
                newRatesByZoneId = null;
                closedRatesByZoneId = null;
                minRateActionDate = null;
                return false;
            }

            newRatesByZoneId = new Dictionary<long, RateToChange>();
            closedRatesByZoneId = new Dictionary<long, RateToClose>();
            minRateActionDate = null;

            foreach (DataByZone zoneData in dataByZoneId.Values)
            {
                if (zoneData.NormalRateToChange != null)
                {
                    if (!minRateActionDate.HasValue || zoneData.NormalRateToChange.BED < minRateActionDate.Value)
                        minRateActionDate = zoneData.NormalRateToChange.BED;
                    newRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToChange);
                }
                else if (zoneData.NormalRateToClose != null)
                {
                    if (!minRateActionDate.HasValue || zoneData.NormalRateToClose.CloseEffectiveDate < minRateActionDate.Value)
                        minRateActionDate = zoneData.NormalRateToClose.CloseEffectiveDate;
                    closedRatesByZoneId.Add(zoneData.ZoneId, zoneData.NormalRateToClose);
                }
            }

            return minRateActionDate.HasValue;
        }
        private static IEnumerable<ExistingZone> GetEffectiveOrFutureZones(IEnumerable<ExistingZone> existingZones, string countryName, DateTime countryBED)
        {
            if (existingZones == null || existingZones.Count() == 0)
                throw new Vanrise.Entities.VRBusinessException(string.Format("The existing zones of country '{0}' were not found", countryName));

            IEnumerable<ExistingZone> effectiveOrFutureZones = existingZones.FindAllRecords(x => x.IsEffectiveOrFuture(countryBED));

            if (effectiveOrFutureZones.Count() == 0)
                throw new Vanrise.Entities.VRBusinessException(string.Format("No effective or future zones of country '{0}' were found after the country's BED '{1}'", countryName, UtilitiesManager.GetDateTimeAsString(countryBED)));

            return effectiveOrFutureZones;
        }
        private static IEnumerable<ExistingRate> GetEffectiveOrFutureRates(IEnumerable<ExistingRate> existingRates, DateTime countryBED)
        {
            if (existingRates == null || existingRates.Count() == 0)
                return null;
            return existingRates.FindAllRecords(x => x.IsEffectiveOrFuture(countryBED));
        }
        private static bool AreZoneRatesInConflict(IEnumerable<ExistingRate> existingRates, DateTime xDate, DateTime? yDate, IRatePlanContext ratePlanContext)
        {
            if (existingRates == null || existingRates.Count() == 0)
            {
                if (AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                    return true;
            }
            else
            {
                var saleRateManager = new SaleRateManager();
                DateTime? previousRateEED = null;

                int existingRatesCount = existingRates.Count();

                for (int i = 0; i < existingRatesCount; i++)
                {
                    ExistingRate existingRate = existingRates.ElementAt(i);

                    if (i == 0)
                    {
                        if (existingRate.BED > xDate && AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                            return true;
                    }
                    else if (existingRate.BED > previousRateEED.Value && AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                        return true;

                    int existingRateCurrencyId = saleRateManager.GetCurrencyId(existingRate.RateEntity);

                    if (existingRateCurrencyId != ratePlanContext.CurrencyId)
                        return true;

                    if (!existingRate.EED.HasValue || (yDate.HasValue && existingRate.EED.Value >= yDate.Value))
                        break;

                    if (i == existingRatesCount - 1)
                    {
                        if (AreProcessAndSystemCurrenciesInConflict(ratePlanContext))
                            return false;
                    }

                    previousRateEED = existingRate.EED;
                }
            }

            return false;
        }
        private static bool AreProcessAndSystemCurrenciesInConflict(IRatePlanContext ratePlanContext)
        {
            return ratePlanContext.CurrencyId != ratePlanContext.SellingProductCurrencyId;
        }
        private static DateTime? Max(params DateTime?[] dates)
        {
            if (dates == null || dates.Count() == 0)
                return default(DateTime);
            else
            {
                int datesCount = dates.Count();
                DateTime? firstDate = dates.ElementAt(0);

                if (datesCount == 1)
                    return firstDate;
                else
                {
                    DateTime? maxDate = firstDate;

                    for (int i = 1; i < dates.Count(); i++)
                    {
                        DateTime? date = dates.ElementAt(i);

                        if (!date.HasValue)
                            continue;

                        if (!maxDate.HasValue || date.Value > maxDate.Value)
                            maxDate = date;
                    }

                    return maxDate;
                }
            }
        }
        private static DateTime? Min(params DateTime?[] dates)
        {
            if (dates == null || dates.Count() == 0)
                return default(DateTime);
            else
            {
                int datesCount = dates.Count();
                DateTime? firstDate = dates.ElementAt(0);

                if (datesCount == 1)
                    return firstDate;
                else
                {
                    DateTime? maxDate = firstDate;

                    for (int i = 1; i < dates.Count(); i++)
                    {
                        DateTime? date = dates.ElementAt(i);

                        if (!date.HasValue)
                            continue;

                        if (!maxDate.HasValue || date.Value < maxDate.Value)
                            maxDate = date;
                    }

                    return maxDate;
                }
            }
        }
        #endregion

        #region Private Methods
        private static void AddRateTypeName(List<string> rateTypeNames, int rateTypeId, Vanrise.Common.Business.RateTypeManager rateTypeManager)
        {
            string rateTypeName = rateTypeManager.GetRateTypeName(rateTypeId);
            if (rateTypeName != null)
                rateTypeNames.Add(rateTypeName);
        }
        private static bool IsRateGreaterThanMaxRate(RateToChange rateToChange, IRatePlanContext ratePlanContext)
        {
            int rateToChangeCurrencyId = ratePlanContext.GetRateToChangeCurrencyId(rateToChange);
            decimal convertedMaximumRate = ratePlanContext.GetMaximumRateConverted(rateToChangeCurrencyId);
            return rateToChange.NormalRate > convertedMaximumRate;
        }
        #endregion
    }
}
