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
    public class CustomerZoneRateHistoryLocatorV2
    {
        #region Fields
        private CustomerZoneRateHistoryReaderV2 _reader;
        private CustomerCountryManager _customerCountryManager;
        private IEnumerable<SaleEntityZoneRateSource> _orderedRateSources;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryLocatorV2(CustomerZoneRateHistoryReaderV2 reader)
        {
            InitializeFields(reader);
        }
        #endregion

        public IEnumerable<SaleRateHistoryRecord> GetCustomerZoneRateHistory(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, int targetCurrencyId, int longPrecision)
        {
            var rateHistoryBySource = new SaleRateHistoryBySource();
            List<CustomerCountry2> customerCountries = GetAllCustomerCountries(customerId, countryId);

            if (customerCountries == null || customerCountries.Count == 0)
                return null;

            AddProductZoneRateHistory(rateHistoryBySource, sellingProductId, zoneName, rateTypeId, customerCountries);
            AddCustomerZoneRateHistory(rateHistoryBySource, customerId, zoneName, rateTypeId);

            return RateHistoryUtilities.GetZoneRateHistory(rateHistoryBySource, _orderedRateSources, targetCurrencyId, longPrecision);
        }
        public SaleRateHistoryRecord GetCustomerZoneRateHistoryRecord(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, DateTime effectiveOn, int targetCurrencyId, int longPrecision)
        {
            IEnumerable<SaleRateHistoryRecord> customerZoneRateHistory = GetCustomerZoneRateHistory(customerId, sellingProductId, zoneName, rateTypeId, countryId, targetCurrencyId, longPrecision);
            return (customerZoneRateHistory != null) ? customerZoneRateHistory.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }

        #region Private Methods
        private void InitializeFields(CustomerZoneRateHistoryReaderV2 reader)
        {
            _reader = reader;
            _customerCountryManager = new CustomerCountryManager();
            _orderedRateSources = new List<SaleEntityZoneRateSource>() { SaleEntityZoneRateSource.ProductZone, SaleEntityZoneRateSource.CustomerZone };
        }
        private List<CustomerCountry2> GetAllCustomerCountries(int customerId, int countryId)
        {
            IEnumerable<CustomerCountry2> allCustomerCountries = _customerCountryManager.GetCustomerCountries(customerId);
            //ThrowIfNullOrEmpty(allCustomerCountries, "allCustomerCountries");

            if (allCustomerCountries == null || allCustomerCountries.Count() == 0)
                return null;

            IEnumerable<CustomerCountry2> customerCountries = allCustomerCountries.FindAllRecords(x => x.CountryId == countryId);
            //ThrowIfNullOrEmpty(customerCountries, "customerCountries");

            if (customerCountries == null || customerCountries.Count() == 0)
                return null;

            return customerCountries.OrderBy(x => x.BED).ToList();
        }
        private void ThrowIfNullOrEmpty<T>(IEnumerable<T> list, string errorMessage)
        {
            if (list == null || list.Count() == 0)
                throw new NullReferenceException(errorMessage);
        }
        private void AddProductZoneRateHistory(SaleRateHistoryBySource rateHistoryBySource, int sellingProductId, string zoneName, int? rateTypeId, List<CustomerCountry2> customerCountries)
        {
            IEnumerable<SaleRate> productZoneRates = _reader.GetProductZoneRates(sellingProductId, zoneName, rateTypeId);

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

            IEnumerable<SaleRateHistoryRecord> productZoneRateHistory = Utilities.GetQIntersectT(customerCountries, productZoneRates.ToList(), mapSaleRate);

            if (productZoneRateHistory != null && productZoneRateHistory.Count() > 0)
                rateHistoryBySource.AddSaleRateHistoryRange(SaleEntityZoneRateSource.ProductZone, productZoneRateHistory);
        }
        private void AddCustomerZoneRateHistory(SaleRateHistoryBySource rateHistoryBySource, int customerId, string zoneName, int? rateTypeId)
        {
            IEnumerable<SaleRate> customerZoneRates = _reader.GetCustomerZoneRates(customerId, zoneName, rateTypeId);

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
        #endregion
    }
}
