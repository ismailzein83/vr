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
    }
}
