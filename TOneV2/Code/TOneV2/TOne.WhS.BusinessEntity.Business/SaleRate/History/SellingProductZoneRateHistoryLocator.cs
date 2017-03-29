using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SellingProductZoneRateHistoryLocator
    {
        #region Fields / Constructors

        private SellingProductZoneRateHistoryReader _reader;

        public SellingProductZoneRateHistoryLocator(SellingProductZoneRateHistoryReader reader)
        {
            _reader = reader;
        }

        #endregion

        public IEnumerable<SaleRateHistoryRecord> GetSaleRateHistory(string zoneName, int? rateTypeId, int currencyId)
        {
            IEnumerable<SaleRate> spZoneRates = _reader.GetSaleRates(zoneName, rateTypeId);
            return GetPreparedSaleRateHistoryRecords(spZoneRates, currencyId);
        }

        public SaleRateHistoryRecord GetSaleRateHistoryRecord(string zoneName, int? rateTypeId, int currencyId, DateTime effectiveOn)
        {
            IEnumerable<SaleRateHistoryRecord> saleRateHistoryRecords = GetSaleRateHistory(zoneName, rateTypeId, currencyId);
            return (saleRateHistoryRecords != null) ? saleRateHistoryRecords.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }

        #region Private Methods

        private IEnumerable<SaleRateHistoryRecord> GetPreparedSaleRateHistoryRecords(IEnumerable<SaleRate> spZoneRates, int currencyId)
        {
            if (spZoneRates == null || spZoneRates.Count() == 0)
                return null;

            var records = new List<SaleRateHistoryRecord>();
            IEnumerable<SaleRate> orderedSPZoneRates = spZoneRates.OrderBy(x => x.BED);

            var saleRateManager = new SaleRateManager();
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            for (int i = 0; i < orderedSPZoneRates.Count(); i++)
            {
                SaleRate rate = orderedSPZoneRates.ElementAt(i);

                decimal? previousRateValue = null;
                if (i > 0) previousRateValue = orderedSPZoneRates.ElementAt(i - 1).Rate;

                var record = new SaleRateHistoryRecord()
                {
                    Rate = rate.Rate,
                    CurrencyId = saleRateManager.GetCurrencyId(rate),
                    BED = rate.BED,
                    EED = rate.EED
                };

                record.ConvertedRate = currencyExchangeRateManager.ConvertValueToCurrency(record.Rate, record.CurrencyId, currencyId, record.BED);
                record.ChangeType = GetSaleRateChangeType(record.ConvertedRate, previousRateValue);

                records.Add(record);
            }

            return records;
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
    }
}
