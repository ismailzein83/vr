using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ProductZoneRateHistoryLocator
    {
        #region Fields
        private ProductZoneRateHistoryReader _reader;
        #endregion

        #region Constructors
        public ProductZoneRateHistoryLocator(ProductZoneRateHistoryReader reader)
        {
            InitializeFields(reader);
        }
        #endregion

        public IEnumerable<SaleRateHistoryRecord> GetProductZoneRateHistory(int sellingProductId, string zoneName, int? rateTypeId, int targetCurrencyId)
        {
            IEnumerable<SaleRate> productZoneRates = _reader.GetProductZoneRates(sellingProductId, zoneName, rateTypeId);

            if (productZoneRates == null || productZoneRates.Count() == 0)
                return null;

            var saleRateManager = new SaleRateManager();
            var currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();

            return productZoneRates.MapRecords((saleRate) =>
            {
                int rateCurrencyId = saleRateManager.GetCurrencyId(saleRate);
                return new SaleRateHistoryRecord()
                {
                    SaleRateId = saleRate.SaleRateId,
                    Rate = saleRate.Rate,
                    ConvertedRate = currencyExchangeRateManager.ConvertValueToCurrency(saleRate.Rate, rateCurrencyId, targetCurrencyId, saleRate.BED),
                    PriceListId = saleRate.PriceListId,
                    ChangeType = saleRate.RateChange,
                    CurrencyId = rateCurrencyId,
                    SellingProductId = null,
                    BED = saleRate.BED,
                    EED = saleRate.EED,
                    SourceId = saleRate.SourceId
                };
            });
        }
        public SaleRateHistoryRecord GetProductZoneRateHistoryRecord(int sellingProductId, string zoneName, int? rateTypeId, int targetCurrencyId, DateTime effectiveOn)
        {
            IEnumerable<SaleRateHistoryRecord> productZoneRateHistory = GetProductZoneRateHistory(sellingProductId, zoneName, rateTypeId, targetCurrencyId);
            return (productZoneRateHistory != null) ? productZoneRateHistory.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }

        #region Private Methods
        private void InitializeFields(ProductZoneRateHistoryReader reader)
        {
            _reader = reader;
        }
        #endregion
    }
}
