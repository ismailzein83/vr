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
        #region Fields
        private CustomerZoneRateHistoryReader _reader;
        private CustomerCountryManager _customerCountryManager;
        private IEnumerable<SaleEntityZoneRateSource> _orderedRateSources;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryLocator(CustomerZoneRateHistoryReader reader)
        {
            InitializeFields(reader);
        }
        #endregion

        public IEnumerable<SaleRateHistoryRecord> GetCustomerZoneRateHistory(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, int? targetCurrencyId, int? longPrecision)
        {
            var rateHistoryBySource = new SaleRateHistoryBySource();
            List<CustomerCountry2> customerCountries = RateHistoryUtilities.GetAllCustomerCountries(customerId, countryId);

            if (customerCountries == null || customerCountries.Count == 0)
                return null;

            List<TimeInterval> customerCountryTimeIntervals = customerCountries.MapRecords(RateHistoryUtilities.CountryTimeIntervalMapper).ToList();
            RateHistoryUtilities.AddProductZoneRateHistory(rateHistoryBySource, sellingProductId, zoneName, rateTypeId, customerCountryTimeIntervals, _reader);
            RateHistoryUtilities.AddCustomerZoneRateHistory(rateHistoryBySource, customerId, zoneName, rateTypeId, _reader);

            return RateHistoryUtilities.GetZoneRateHistory(rateHistoryBySource, _orderedRateSources, targetCurrencyId, longPrecision);
        }
        public SaleRateHistoryRecord GetCustomerZoneRateHistoryRecord(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, DateTime effectiveOn, int targetCurrencyId, int longPrecision)
        {
            IEnumerable<SaleRateHistoryRecord> customerZoneRateHistory = GetCustomerZoneRateHistory(customerId, sellingProductId, zoneName, rateTypeId, countryId, targetCurrencyId, longPrecision);
            return (customerZoneRateHistory != null) ? customerZoneRateHistory.FindRecord(x => x.IsEffective(effectiveOn)) : null;
        }
        public SaleRateHistoryRecord GetLastCustomerRateHistoryRecord(int customerId, int sellingProductId, string zoneName, int? rateTypeId, int countryId, int? targetCurrencyId, int? longPrecision)
        {
            var customerZoneRateHistory = GetCustomerZoneRateHistory(customerId, sellingProductId, zoneName, rateTypeId, countryId, targetCurrencyId, longPrecision);
            if (customerZoneRateHistory != null)
                return customerZoneRateHistory.LastOrDefault();
            return null;
        }

        #region Private Methods
        private void InitializeFields(CustomerZoneRateHistoryReader reader)
        {
            _reader = reader;
            _customerCountryManager = new CustomerCountryManager();
            _orderedRateSources = new List<SaleEntityZoneRateSource>() { SaleEntityZoneRateSource.ProductZone, SaleEntityZoneRateSource.CustomerZone };
        }
        #endregion
    }
}
