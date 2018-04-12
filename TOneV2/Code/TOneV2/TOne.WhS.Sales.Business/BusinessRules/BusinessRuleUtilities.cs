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

        #region Validate Country Zone Rate Currencies

        public static bool ValidateCountryZoneRateCurrencies(CountryData countryData, IRatePlanContext ratePlanContext, int sellingProductId, out string errorMessage)
        {
            IEnumerable<ExistingZone> countryZones = ratePlanContext.EffectiveAndFutureExistingZonesByCountry.GetRecord(countryData.CountryId);

            if (countryZones == null || countryZones.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException("countryZones");

            var newRates = new List<RateToChange>();
            var closedRates = new List<RateToClose>();
            var zonesWithoutRateActions = new List<ExistingZone>();
            DateTime? minRateActionDate;

            if (!DoRateActionsExist(countryZones, countryData.ZoneDataByZoneId, newRates, closedRates, zonesWithoutRateActions, out minRateActionDate))
            {
                errorMessage = null;
                return true;
            }

            string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);

            if (newRates.Count > 0 && closedRates.Count > 0 && ratePlanContext.CurrencyId != ratePlanContext.SellingProductCurrencyId)
            {
                var currencyManager = new Vanrise.Common.Business.CurrencyManager();
                errorMessage = string.Format("Cannot define new rates and close others for country '{0}' when the process's currency '{1}' is different than that of the selling product '{2}'", countryName, currencyManager.GetCurrencySymbol(ratePlanContext.CurrencyId), currencyManager.GetCurrencySymbol(ratePlanContext.SellingProductCurrencyId));
                return false;
            }

            int targetCurrencyId = (closedRates.Count > 0) ? ratePlanContext.SellingProductCurrencyId : ratePlanContext.CurrencyId;

            if
            (
                ValidateNewRates(newRates, ratePlanContext, minRateActionDate.Value, targetCurrencyId, sellingProductId, countryData, countryZones) &&
                ValidateClosedRates(closedRates, ratePlanContext, minRateActionDate.Value, targetCurrencyId, sellingProductId, countryData) &&
                ValidateZonesWithoutRateActions(zonesWithoutRateActions, ratePlanContext, minRateActionDate.Value, targetCurrencyId, sellingProductId, countryData)
            )
            {
                errorMessage = null;
                return true;
            }

            errorMessage = string.Format("Rate actions taken on country '{0}' violate currency constraints", countryName);
            return false;
        }

        #region Methods
        private static bool DoRateActionsExist(IEnumerable<ExistingZone> countryZones, Dictionary<long, DataByZone> zoneDataByZoneId, List<RateToChange> newRates, List<RateToClose> closedRates, List<ExistingZone> zonesWithoutRateActions, out DateTime? minRateActionDate)
        {
            if (zoneDataByZoneId == null || zoneDataByZoneId.Count == 0)
            {
                minRateActionDate = null;
                return false;
            }

            DateTime? minRateActionDateValue = null;
            Action<DateTime> updateMinRateActionDateValue = (date) => { if (!minRateActionDateValue.HasValue || date < minRateActionDateValue.Value) { minRateActionDateValue = date; } };

            foreach (ExistingZone countryZone in countryZones)
            {
                DataByZone zoneData;

                if (!zoneDataByZoneId.TryGetValue(countryZone.ZoneId, out zoneData))
                    zonesWithoutRateActions.Add(countryZone);
                else
                {
                    if (zoneData.NormalRateToChange != null)
                    {
                        updateMinRateActionDateValue(zoneData.NormalRateToChange.BED);
                        newRates.Add(zoneData.NormalRateToChange);
                    }
                    else if (zoneData.NormalRateToClose != null)
                    {
                        updateMinRateActionDateValue(zoneData.NormalRateToClose.CloseEffectiveDate);
                        closedRates.Add(zoneData.NormalRateToClose);
                    }
                }
            }

            minRateActionDate = minRateActionDateValue;
            return minRateActionDateValue.HasValue;
        }
        private static bool ValidateNewRates(IEnumerable<RateToChange> newRates, IRatePlanContext ratePlanContext, DateTime minRateActionDate, int targetCurrencyId, int sellingProductId, CountryData countryData, IEnumerable<ExistingZone> countryZones)
        {
            if (newRates.Count() == 0)
                return true;

            if (ratePlanContext.CurrencyId != targetCurrencyId)
                return false;

            foreach (RateToChange newRate in newRates)
            {
                var newRateZone = countryZones.FindRecord(item => item.ZoneId == newRate.ZoneId);
                if (newRateZone == null)
                    throw new DataIntegrityValidationException(string.Format("Define new rate for not sold zone {0}", newRate.ZoneName));

                if (newRate.BED == Utilities.Max(newRateZone.BED, countryData.CountryBED))
                    continue;

                if (newRate.BED == minRateActionDate)
                    continue;

                IEnumerable<SaleRateHistoryRecord> zoneRateHistory = GetCustomerZoneRateHistory(ratePlanContext.OwnerId, sellingProductId, newRate.ZoneName, newRate.RateTypeId, countryData.CountryId, targetCurrencyId, ratePlanContext.LongPrecision, ratePlanContext.CustomerZoneRateHistoryReader, null);

                if (zoneRateHistory == null || zoneRateHistory.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException("zoneRateHistory");

                if (!ValidateZoneRateHistoryWithinTimeInterval(zoneRateHistory, minRateActionDate, newRate.BED, targetCurrencyId, newRate.RateTypeId, newRate.ZoneId))
                    return false;
            }

            return true;
        }
        private static bool ValidateClosedRates(IEnumerable<RateToClose> closedRates, IRatePlanContext ratePlanContext, DateTime minRateActionDate, int targetCurrencyId, int sellingProductId, CountryData countryData)
        {
            foreach (RateToClose closedRate in closedRates)
            {
                if (closedRate.CloseEffectiveDate == minRateActionDate)
                    continue;

                IEnumerable<SaleRateHistoryRecord> zoneRateHistory = GetCustomerZoneRateHistory(ratePlanContext.OwnerId, sellingProductId, closedRate.ZoneName, closedRate.RateTypeId, countryData.CountryId, targetCurrencyId, ratePlanContext.LongPrecision, ratePlanContext.CustomerZoneRateHistoryReader, null);

                if (zoneRateHistory == null || zoneRateHistory.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException("zoneRateHistory");

                if (!ValidateZoneRateHistoryWithinTimeInterval(zoneRateHistory, minRateActionDate, closedRate.CloseEffectiveDate, targetCurrencyId, closedRate.RateTypeId, closedRate.ZoneId))
                    return false;
            }

            return true;
        }
        private static bool ValidateZonesWithoutRateActions(IEnumerable<ExistingZone> zonesWithoutRateActions, IRatePlanContext ratePlanContext, DateTime minRateActionDate, int targetCurrencyId, int sellingProductId, CountryData countryData)
        {
            foreach (ExistingZone zoneWithoutRateActions in zonesWithoutRateActions)
            {
                if (zoneWithoutRateActions.EED.HasValue && minRateActionDate >= zoneWithoutRateActions.EED.Value)
                    continue;
                IEnumerable<int> zoneOtherRateTypeIds = Helper.GetRateTypeIds(ratePlanContext.OwnerId, zoneWithoutRateActions.ZoneId, ratePlanContext.EffectiveDate);

                if (!ValidateZoneWithoutRateActions(zoneWithoutRateActions, ratePlanContext, minRateActionDate, targetCurrencyId, sellingProductId, countryData, zoneOtherRateTypeIds))
                    return false;
            }

            return true;
        }
        private static bool ValidateZoneWithoutRateActions(ExistingZone zoneWithoutRateActions, IRatePlanContext ratePlanContext, DateTime minRateActionDate, int targetCurrencyId, int sellingProductId, CountryData countryData, IEnumerable<int> zoneOtherRateTypeIds)
        {
            TimeInterval timeInterval = null;
            DateTime zoneEffectiveDate = Utilities.Max(countryData.CountryBED, zoneWithoutRateActions.ZoneEntity.BED);
            DateTime timeIntervalBED = Utilities.Max(minRateActionDate, zoneEffectiveDate);
            if (countryData.IsCountryNew)
            {
                timeInterval = new TimeInterval() { BED = zoneEffectiveDate };
            }
            IEnumerable<SaleRateHistoryRecord> zoneNormalRateHistory = GetCustomerZoneRateHistory(ratePlanContext.OwnerId, sellingProductId, zoneWithoutRateActions.Name, null, countryData.CountryId, targetCurrencyId, ratePlanContext.LongPrecision, ratePlanContext.CustomerZoneRateHistoryReader, timeInterval);

            if (zoneNormalRateHistory == null || zoneNormalRateHistory.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Zone with id {0} rate history was not found.", zoneWithoutRateActions.ZoneId));

            if (!ValidateZoneRateHistoryWithinTimeInterval(zoneNormalRateHistory, timeIntervalBED, null, targetCurrencyId, null, zoneWithoutRateActions.ZoneId))
                return false;

            if (zoneOtherRateTypeIds == null || zoneOtherRateTypeIds.Count() == 0)
                return true;

            foreach (int zoneOtherRateTypeId in zoneOtherRateTypeIds)
            {
                IEnumerable<SaleRateHistoryRecord> zoneOtherRateHistory = GetCustomerZoneRateHistory(ratePlanContext.OwnerId, sellingProductId, zoneWithoutRateActions.Name, zoneOtherRateTypeId, countryData.CountryId, targetCurrencyId, ratePlanContext.LongPrecision, ratePlanContext.CustomerZoneRateHistoryReader, timeInterval);

                if (zoneOtherRateHistory == null || zoneOtherRateHistory.Count() == 0)
                    continue;

                if (!ValidateZoneRateHistoryWithinTimeInterval(zoneOtherRateHistory, timeIntervalBED, null, targetCurrencyId, zoneOtherRateTypeId, zoneWithoutRateActions.ZoneId))
                    return false;
            }

            return true;
        }
        private static bool ValidateZoneRateHistoryWithinTimeInterval(IEnumerable<SaleRateHistoryRecord> zoneRateHistory, DateTime timeIntervalBED, DateTime? timeIntervalEED, int targetCurrencyId, int? rateTypeId, long zoneId)
        {
            var timeInterval = new TimeInterval() { BED = timeIntervalBED, EED = timeIntervalEED };
            IEnumerable<SaleRateHistoryRecord> targetZoneRateHistory = Vanrise.Common.Utilities.GetQIntersectT(ToList(timeInterval), zoneRateHistory.ToList(), ZoneRateHistoryMapper);

            if (!rateTypeId.HasValue && (targetZoneRateHistory == null || targetZoneRateHistory.Count() == 0))
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Zone with id '{0}' rate history target was not found.", zoneId));

            foreach (SaleRateHistoryRecord saleRateHistoryRecord in targetZoneRateHistory)
            {
                if (saleRateHistoryRecord.CurrencyId != targetCurrencyId)
                    return false;
            }

            return true;
        }
        private static List<T> ToList<T>(T item)
        {
            return new List<T>() { item };
        }
        #endregion

        #region Mappers
        private static Action<SaleRateHistoryRecord, SaleRateHistoryRecord> ZoneRateHistoryMapper = (saleRateHistoryRecord, saleRateHistoryRecordOutput) =>
        {
            saleRateHistoryRecordOutput.SaleRateId = saleRateHistoryRecord.SaleRateId;
            saleRateHistoryRecordOutput.Rate = saleRateHistoryRecord.Rate;
            saleRateHistoryRecordOutput.ConvertedRate = saleRateHistoryRecord.ConvertedRate;
            saleRateHistoryRecordOutput.PriceListId = saleRateHistoryRecord.PriceListId;
            saleRateHistoryRecordOutput.ChangeType = saleRateHistoryRecord.ChangeType;
            saleRateHistoryRecordOutput.CurrencyId = saleRateHistoryRecord.CurrencyId;
            saleRateHistoryRecordOutput.SellingProductId = saleRateHistoryRecord.SellingProductId;
            saleRateHistoryRecordOutput.SourceId = saleRateHistoryRecord.SourceId;
        };
        #endregion

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

        private static IEnumerable<SaleRateHistoryRecord> GetCustomerZoneRateHistory(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, int targetCurrencyId, int longPrecision, CustomerZoneRateHistoryReader reader, TimeInterval timeInterval)
        {
            var orderedRateSources = new List<SaleEntityZoneRateSource>() { SaleEntityZoneRateSource.ProductZone, SaleEntityZoneRateSource.CustomerZone };
            var rateHistoryBySource = new SaleRateHistoryBySource();

            List<TimeInterval> customerCountryTimeIntervals = new List<TimeInterval>();
            if (timeInterval == null)
                customerCountryTimeIntervals = RateHistoryUtilities.GetAllCustomerCountries(customerId, countryId).MapRecords(RateHistoryUtilities.CountryTimeIntervalMapper).ToList();
            else customerCountryTimeIntervals.Add(timeInterval);

            RateHistoryUtilities.AddProductZoneRateHistory(rateHistoryBySource, sellingProductId, zoneName, rateTypeId, customerCountryTimeIntervals, reader);
            RateHistoryUtilities.AddCustomerZoneRateHistory(rateHistoryBySource, customerId, zoneName, rateTypeId, reader);

            return RateHistoryUtilities.GetZoneRateHistory(rateHistoryBySource, orderedRateSources, targetCurrencyId, longPrecision);
        }


        #endregion
    }
}
