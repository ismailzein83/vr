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
        public static SaleRateHistoryRecord GetSaleRateHistoryRecord(int customerId, string zoneName, int countryId, int? rateTypeId, int targetCurrencyId, DateTime effectiveOn, CustomerZoneRateHistoryReader reader)
        {
            IEnumerable<SaleRateHistoryRecord> saleRateHistoryRecords = GetSaleRateHistory(customerId, zoneName, countryId, rateTypeId, targetCurrencyId, reader);
            return (saleRateHistoryRecords != null) ? saleRateHistoryRecords.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }
        public static IEnumerable<SaleRateHistoryRecord> GetSaleRateHistory(int customerId, string zoneName, int countryId, int? rateTypeId, int targetCurrencyId, CustomerZoneRateHistoryReader reader)
        {
            IEnumerable<SaleRateHistoryRecord> countryIntersectedRates = null;
            IEnumerable<SaleRateHistoryRecord> customerIntersectedRates = null;

            int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);

            List<SaleRate> spZoneRates;
            IEnumerable<SaleRate> customerZoneRates;
            SetSPAndCustomerZoneRates(zoneName, rateTypeId, reader, out spZoneRates, out customerZoneRates);

            var saleRateManager = new SaleRateManager();

            IEnumerable<SaleRateHistoryRecord> mappedSPZoneRates = (spZoneRates != null) ? spZoneRates.MapRecords(x => new SaleRateHistoryRecord()
            {
                SaleRateId = x.SaleRateId,
                Rate = x.Rate,
                PriceListId = x.PriceListId,
                CurrencyId = saleRateManager.GetCurrencyId(x),
                SellingProductId = sellingProductId,
                SourceId = x.SourceId
            }) : null;

            if (mappedSPZoneRates != null && mappedSPZoneRates.Count() > 0)
            {
                IEnumerable<CustomerCountry2> countryPeriods = reader.GetCountryPeriods(countryId);
                countryIntersectedRates = GetCountryIntersectedRates(countryPeriods, mappedSPZoneRates);
            }

            customerIntersectedRates = GetCustomerIntersectedRates(customerZoneRates, countryIntersectedRates, saleRateManager);

            if (customerIntersectedRates == null || customerIntersectedRates.Count() == 0)
                return null;

            List<SaleRateHistoryRecord> customerIntersectedRatesAsList = customerIntersectedRates.ToList();
            PrepareSaleRateHistoryRecords(customerIntersectedRatesAsList, targetCurrencyId);

            return customerIntersectedRatesAsList;
        }

        #region Private Methods
        private static void SetSPAndCustomerZoneRates(string zoneName, int? rateTypeId, CustomerZoneRateHistoryReader reader, out List<SaleRate> spZoneRates, out IEnumerable<SaleRate> customerZoneRates)
        {
            spZoneRates = null;
            customerZoneRates = null;

            CustomerZoneRates zoneRates = reader.GetZoneRates(zoneName);

            if (zoneRates != null)
            {
                RateTypeKey rateTypeKey = new RateTypeKey() { RateTypeId = rateTypeId };
                spZoneRates = zoneRates.SellingProductZoneRatesByType.GetRecord(rateTypeKey);
                customerZoneRates = zoneRates.CustomerZoneRatesByType.GetRecord(rateTypeKey);
            }
        }
        private static IEnumerable<SaleRateHistoryRecord> GetCountryIntersectedRates(IEnumerable<CustomerCountry2> customerCountries, IEnumerable<SaleRateHistoryRecord> spZoneRates)
        {
            List<CustomerCountry2> countriesAsList = customerCountries.ToList();
            List<SaleRateHistoryRecord> spZoneRatesAsList = spZoneRates.ToList();
            return Vanrise.Common.Utilities.GetQIntersectT(countriesAsList, spZoneRatesAsList, MapSaleRateHistoryRecordAction);
        }
        private static IEnumerable<SaleRateHistoryRecord> GetCustomerIntersectedRates(IEnumerable<SaleRate> customerRates, IEnumerable<SaleRateHistoryRecord> countryIntersectedRates, SaleRateManager saleRateManager)
        {
            if (customerRates == null || customerRates.Count() == 0)
                return countryIntersectedRates;

            if (countryIntersectedRates == null || countryIntersectedRates.Count() == 0)
            {
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
                return customerRates.MapRecords(saleRateMapper);
            }

            List<SaleRate> customerRatesAsList = customerRates.OrderBy(x => x.BED).ToList();
            List<SaleRateHistoryRecord> countryIntersectedRatesAsList = (countryIntersectedRates != null) ? countryIntersectedRates.ToList() : null;

            Action<SaleRate, SaleRateHistoryRecord> mapSaleRateAction = (rate, record) =>
            {
                record.SaleRateId = rate.SaleRateId;
                record.Rate = rate.Rate;
                record.PriceListId = rate.PriceListId;
                record.CurrencyId = saleRateManager.GetCurrencyId(rate);
                record.SellingProductId = null;
                record.SourceId = rate.SourceId;
            };

            return Vanrise.Common.Utilities.MergeUnionWithQForce(countryIntersectedRatesAsList, customerRatesAsList, MapSaleRateHistoryRecordAction, mapSaleRateAction);
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

        #region Mappers
        private static Action<SaleRateHistoryRecord, SaleRateHistoryRecord> MapSaleRateHistoryRecordAction = (record, targetRecord) =>
        {
            targetRecord.SaleRateId = record.SaleRateId;
            targetRecord.Rate = record.Rate;
            targetRecord.PriceListId = record.PriceListId;
            targetRecord.ChangeType = record.ChangeType;
            targetRecord.CurrencyId = record.CurrencyId;
            targetRecord.SellingProductId = record.SellingProductId;
            targetRecord.SourceId = record.SourceId;
        };
        #endregion

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
    }
}
