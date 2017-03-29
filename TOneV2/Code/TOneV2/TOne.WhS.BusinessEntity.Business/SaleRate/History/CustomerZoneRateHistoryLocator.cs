using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRateHistoryLocator
    {
        #region Fields / Constructors

        private CustomerZoneRateHistoryReader _reader;
        private SaleRateManager _saleRateManager = new SaleRateManager();

        #endregion

        public CustomerZoneRateHistoryLocator(CustomerZoneRateHistoryReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<SaleRateHistoryRecord> GetSaleRateHistory(string zoneName, int countryId, int? rateTypeId, int currencyId)
        {
            IEnumerable<SaleRateHistoryRecord> spIntersectedRates = null;
            IEnumerable<SaleRateHistoryRecord> countryIntersectedRates = null;
            IEnumerable<SaleRateHistoryRecord> customerIntersectedRates = null;

            IEnumerable<int> sellingProductIds;
            Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP = _reader.GetSellingProductAssignments(out sellingProductIds);

            Dictionary<int, List<SaleRate>> spZoneRatesBySP;
            IEnumerable<SaleRate> customerZoneRates;
            SetSPAndCustomerZoneRates(zoneName, rateTypeId, out spZoneRatesBySP, out customerZoneRates);

            spIntersectedRates = GetSPIntersectedRates(sellingProductIds, spAssignmentsBySP, spZoneRatesBySP);

            if (spIntersectedRates != null && spIntersectedRates.Count() > 0)
            {
                IEnumerable<CustomerCountry2> countryPeriods = _reader.GetCountryPeriods(countryId);
                countryIntersectedRates = GetCountryIntersectedRates(countryPeriods, spIntersectedRates);
            }

            customerIntersectedRates = GetCustomerIntersectedRates(customerZoneRates, countryIntersectedRates);

            if (customerIntersectedRates == null || customerIntersectedRates.Count() == 0)
                return null;

            List<SaleRateHistoryRecord> customerIntersectedRatesAsList = customerIntersectedRates.ToList();
            PrepareSaleRateHistoryRecords(customerIntersectedRatesAsList, currencyId);

            return customerIntersectedRatesAsList;
        }

        public SaleRateHistoryRecord GetSaleRateHistoryRecord(string zoneName, int countryId, int? rateTypeId, int currencyId, DateTime effectiveOn)
        {
            IEnumerable<SaleRateHistoryRecord> saleRateHistoryRecords = GetSaleRateHistory(zoneName, countryId, rateTypeId, currencyId);
            return (saleRateHistoryRecords != null) ? saleRateHistoryRecords.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }

        #region Private Methods

        private void SetSPAndCustomerZoneRates(string zoneName, int? rateTypeId, out Dictionary<int, List<SaleRate>> spZoneRatesBySP, out IEnumerable<SaleRate> customerZoneRates)
        {
            spZoneRatesBySP = null;
            customerZoneRates = null;

            CustomerZoneRates zoneRates = _reader.GetZoneRates(zoneName);

            if (zoneRates != null)
            {
                RateTypeKey rateTypeKey = new RateTypeKey() { RateTypeId = rateTypeId };
                spZoneRatesBySP = zoneRates.SellingProductZoneRatesByType.GetRecord(rateTypeKey);
                customerZoneRates = zoneRates.CustomerZoneRatesByType.GetRecord(rateTypeKey);
            }
        }

        private IEnumerable<SaleRateHistoryRecord> GetSPIntersectedRates(IEnumerable<int> sellingProductIds, Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP, Dictionary<int, List<SaleRate>> spZoneRatesBySP)
        {
            var allIntersectedRecords = new List<SaleRateHistoryRecord>();

            foreach (int sellingProductId in sellingProductIds)
            {
                List<ProcessedCustomerSellingProduct> spAssignments = spAssignmentsBySP.GetRecord(sellingProductId);
                List<SaleRate> spZoneRates = spZoneRatesBySP.GetRecord(sellingProductId);

                if (spZoneRates == null || spZoneRates.Count == 0)
                    continue;

                Action<SaleRate, SaleRateHistoryRecord> MapSaleRateAction = (saleRate, saleRateHistoryRecord) =>
                {
                    saleRateHistoryRecord.SaleRateId = saleRate.SaleRateId;
                    saleRateHistoryRecord.Rate = saleRate.Rate;
                    saleRateHistoryRecord.PriceListId = saleRate.PriceListId;
                    saleRateHistoryRecord.CurrencyId = _saleRateManager.GetCurrencyId(saleRate);
                    saleRateHistoryRecord.SellingProductId = sellingProductId;
                    saleRateHistoryRecord.SourceId = saleRate.SourceId;
                };

                IEnumerable<SaleRateHistoryRecord> intersectedRecords = Vanrise.Common.Utilities.GetQIntersectT(spAssignments, spZoneRates, MapSaleRateAction);

                if (intersectedRecords != null && intersectedRecords.Count() > 0)
                    allIntersectedRecords.AddRange(intersectedRecords);
            }

            return allIntersectedRecords.OrderBy(x => x.BED);
        }

        private IEnumerable<SaleRateHistoryRecord> GetCountryIntersectedRates(IEnumerable<CustomerCountry2> customerCountries, IEnumerable<SaleRateHistoryRecord> spIntersectedRates)
        {
            List<CustomerCountry2> countriesAsList = customerCountries.ToList();
            List<SaleRateHistoryRecord> spIntersectedRatesAsList = spIntersectedRates.ToList();
            return Vanrise.Common.Utilities.GetQIntersectT(countriesAsList, spIntersectedRatesAsList, MapSaleRateHistoryRecordAction);
        }

        private IEnumerable<SaleRateHistoryRecord> GetCustomerIntersectedRates(IEnumerable<SaleRate> customerRates, IEnumerable<SaleRateHistoryRecord> countryIntersectedRates)
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
                        CurrencyId = _saleRateManager.GetCurrencyId(saleRate),
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
                record.CurrencyId = _saleRateManager.GetCurrencyId(rate);
                record.SellingProductId = null;
                record.SourceId = rate.SourceId;
            };

            return Vanrise.Common.Utilities.MergeUnionWithQForce(countryIntersectedRatesAsList, customerRatesAsList, MapSaleRateHistoryRecordAction, mapSaleRateAction);
        }

        private void PrepareSaleRateHistoryRecords(List<SaleRateHistoryRecord> records, int currencyId)
        {
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            for (int i = 0; i < records.Count; i++)
            {
                SaleRateHistoryRecord record = records.ElementAt(i);
                record.ConvertedRate = currencyExchangeRateManager.ConvertValueToCurrency(record.Rate, record.CurrencyId, currencyId, record.BED);

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

        private RateChangeType GetSaleRateChangeType(decimal rateValue, decimal? previousRateValue)
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

        private Action<SaleRateHistoryRecord, SaleRateHistoryRecord> MapSaleRateHistoryRecordAction = (record, targetRecord) =>
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
    }
}
