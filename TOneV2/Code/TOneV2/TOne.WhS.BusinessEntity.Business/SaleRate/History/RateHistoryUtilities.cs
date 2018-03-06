using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RateHistoryUtilities
    {
        #region Get Zone Rate History
        public static IEnumerable<SaleRateHistoryRecord> GetZoneRateHistory(SaleRateHistoryBySource rateHistoryBySource, IEnumerable<SaleEntityZoneRateSource> orderedRateSources, int targetCurrencyId, int longPrecision)
        {
            if (rateHistoryBySource.GetCount() == 0)
                return null;

            OrderedZoneRateHistories orderedZoneRateHistories = GetOrderedZoneRateHistories(rateHistoryBySource, orderedRateSources);

            IEnumerable<SaleRateHistoryRecord> tList;
            IEnumerable<SaleRateHistoryRecord> qList;
            IEnumerable<SaleRateHistoryRecord> rList = orderedZoneRateHistories.GetZoneRateHistory(0);

            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            Action<SaleRateHistoryRecord, SaleRateHistoryRecord> saleRateHistoryRecordMapper = (saleRateHistoryRecord, targetSaleRateHistoryRecord) =>
            {
                targetSaleRateHistoryRecord.SaleRateId = saleRateHistoryRecord.SaleRateId;
                targetSaleRateHistoryRecord.Rate = saleRateHistoryRecord.Rate;

                decimal convertedRate = currencyExchangeRateManager.ConvertValueToCurrency(saleRateHistoryRecord.Rate, saleRateHistoryRecord.CurrencyId, targetCurrencyId, saleRateHistoryRecord.BED);
                targetSaleRateHistoryRecord.ConvertedRate = decimal.Round(convertedRate, longPrecision);

                targetSaleRateHistoryRecord.PriceListId = saleRateHistoryRecord.PriceListId;
                targetSaleRateHistoryRecord.ChangeType = saleRateHistoryRecord.ChangeType;
                targetSaleRateHistoryRecord.CurrencyId = saleRateHistoryRecord.CurrencyId;
                targetSaleRateHistoryRecord.SellingProductId = saleRateHistoryRecord.SellingProductId;
                targetSaleRateHistoryRecord.SourceId = saleRateHistoryRecord.SourceId;
            };

            for (int i = 0; (i + 1) < orderedZoneRateHistories.GetCount(); i++)
            {
                tList = rList;
                qList = orderedZoneRateHistories.GetZoneRateHistory(i + 1);

                List<SaleRateHistoryRecord> tAsList = (tList != null) ? tList.ToList() : null;
                List<SaleRateHistoryRecord> qAsList = (qList != null) ? qList.ToList() : null;

                rList = Utilities.MergeUnionWithQForce(tAsList, qAsList, saleRateHistoryRecordMapper, saleRateHistoryRecordMapper);
            }

            List<SaleRateHistoryRecord> resultList = rList.ToList();
            PrepareSaleRateHistoryRecords(resultList, targetCurrencyId);

            return resultList;
        }
        private static OrderedZoneRateHistories GetOrderedZoneRateHistories(SaleRateHistoryBySource rateHistoryBySource, IEnumerable<SaleEntityZoneRateSource> orderedRateSources)
        {
            var orderedRateHistories = new OrderedZoneRateHistories();

            foreach (SaleEntityZoneRateSource rateSource in orderedRateSources)
            {
                IEnumerable<SaleRateHistoryRecord> rateHistory = rateHistoryBySource.GetSaleRateHistory(rateSource);
                if (rateHistory != null)
                    orderedRateHistories.AddRateHistory(rateHistory);
            }

            return orderedRateHistories;
        }
        private static void PrepareSaleRateHistoryRecords(List<SaleRateHistoryRecord> records, int currencyId)
        {
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
            int longPrecisionValue = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();

            for (int i = 0; i < records.Count; i++)
            {
                SaleRateHistoryRecord record = records.ElementAt(i);
                record.ConvertedRate = currencyExchangeRateManager.ConvertValueToCurrency(record.Rate, record.CurrencyId, currencyId, record.BED);
                record.ConvertedRate = decimal.Round(record.ConvertedRate, longPrecisionValue);

                SaleRateHistoryRecord previousRecord = null;
                decimal? previousConvertedRateValue = null;

                if (i > 0)
                {
                    previousRecord = records.ElementAt(i - 1);
                    previousConvertedRateValue = previousRecord.ConvertedRate;
                }

                record.ChangeType = GetSaleRateChangeType(record.ConvertedRate, previousConvertedRateValue);
            }
        }
        private static RateChangeType GetSaleRateChangeType(decimal rateValue, decimal? previousRateValue)
        {
            if (!previousRateValue.HasValue)
                return RateChangeType.New;
            else if (rateValue > previousRateValue.Value)
                return RateChangeType.Increase;
            else if (rateValue < previousRateValue.Value)
                return RateChangeType.Decrease;
            else
                return RateChangeType.NotChanged;
        }
        #endregion

        public static List<CustomerCountry2> GetAllCustomerCountries(int customerId, int countryId)
        {
            var customerCountryManager = new CustomerCountryManager();
            IEnumerable<CustomerCountry2> allCustomerCountries = customerCountryManager.GetCustomerCountries(customerId);
            //ThrowIfNullOrEmpty(allCustomerCountries, "allCustomerCountries");

            if (allCustomerCountries == null || allCustomerCountries.Count() == 0)
                return null;

            IEnumerable<CustomerCountry2> customerCountries = allCustomerCountries.FindAllRecords(x => x.CountryId == countryId);
            //ThrowIfNullOrEmpty(customerCountries, "customerCountries");

            if (customerCountries == null || customerCountries.Count() == 0)
                return null;

            return customerCountries.OrderBy(x => x.BED).ToList();
        }

        public static void AddProductZoneRateHistory(SaleRateHistoryBySource rateHistoryBySource, int sellingProductId, string zoneName, int? rateTypeId, List<TimeInterval> timeIntervals, CustomerZoneRateHistoryReader reader)
        {
            IEnumerable<SaleRate> productZoneRates = reader.GetProductZoneRates(sellingProductId, zoneName, rateTypeId);

            if (productZoneRates == null || productZoneRates.Count() == 0)
                return;

            var saleRateManager = new SaleRateManager();

            Action<SaleRate, SaleRateHistoryRecord> mapSaleRate = (saleRate, saleRateHistoryRecord) =>
            {
                saleRateHistoryRecord.SaleRateId = saleRate.SaleRateId;
                saleRateHistoryRecord.Rate = saleRate.Rate;
                saleRateHistoryRecord.PriceListId = saleRate.PriceListId;
                saleRateHistoryRecord.CurrencyId = saleRateManager.GetCurrencyId(saleRate);
                saleRateHistoryRecord.SellingProductId = sellingProductId;
                saleRateHistoryRecord.SourceId = saleRate.SourceId;
            };

            IEnumerable<SaleRateHistoryRecord> productZoneRateHistory = Utilities.GetQIntersectT(timeIntervals, productZoneRates.ToList(), mapSaleRate);

            if (productZoneRateHistory != null && productZoneRateHistory.Count() > 0)
                rateHistoryBySource.AddSaleRateHistoryRange(SaleEntityZoneRateSource.ProductZone, productZoneRateHistory);
        }

        public static void AddCustomerZoneRateHistory(SaleRateHistoryBySource rateHistoryBySource, int customerId, string zoneName, int? rateTypeId, CustomerZoneRateHistoryReader reader)
        {
            IEnumerable<SaleRate> customerZoneRates = reader.GetCustomerZoneRates(customerId, zoneName, rateTypeId);

            if (customerZoneRates == null || customerZoneRates.Count() == 0)
                return;

            var saleRateManager = new SaleRateManager();

            Func<SaleRate, SaleRateHistoryRecord> saleRateMapper = (saleRate) =>
            {
                return new SaleRateHistoryRecord()
                {
                    SaleRateId = saleRate.SaleRateId,
                    Rate = saleRate.Rate,
                    PriceListId = saleRate.PriceListId,
                    CurrencyId = saleRateManager.GetCurrencyId(saleRate),
                    SellingProductId = null,
                    BED = saleRate.BED,
                    EED = saleRate.EED,
                    SourceId = saleRate.SourceId
                };
            };

            IEnumerable<SaleRateHistoryRecord> customerZoneRateHistory = customerZoneRates.MapRecords(saleRateMapper);
            rateHistoryBySource.AddSaleRateHistoryRange(SaleEntityZoneRateSource.CustomerZone, customerZoneRateHistory);
        }

        public static TimeInterval CountryTimeIntervalMapper (CustomerCountry2 customerCountry)
        {
            return new TimeInterval()
            {
                BED = customerCountry.BED,
                EED = customerCountry.EED
            };
        }

    }

    public class TimeInterval : Vanrise.Entities.IDateEffectiveSettingsEditable
    {
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
