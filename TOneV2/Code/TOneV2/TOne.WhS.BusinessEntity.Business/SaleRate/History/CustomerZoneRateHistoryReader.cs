using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRateHistoryReader
    {
        #region Fields
        private Dictionary<int, List<CustomerCountry2>> _countryPeriodsByCountry;
        private Dictionary<string, CustomerZoneRates> _ratesByZoneName;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryReader(int customerId, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            _countryPeriodsByCountry = new Dictionary<int, List<CustomerCountry2>>();
            _ratesByZoneName = new Dictionary<string, CustomerZoneRates>();
            SetCountryPeriods(customerId, zoneIds);
            SetRates(customerId, zoneIds, getNormalRates, getOtherRates);
        }
        #endregion

        public IEnumerable<CustomerCountry2> GetCountryPeriods(int countryId)
        {
            return _countryPeriodsByCountry.GetRecord(countryId);
        }
        public CustomerZoneRates GetZoneRates(string zoneName)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
                throw new Vanrise.Entities.MissingArgumentValidationException("zoneName");

            string zoneNameKey = getZoneNameKey(zoneName);
            return _ratesByZoneName.GetRecord(zoneNameKey);
        }

        #region Private Methods
        private void SetCountryPeriods(int customerId, IEnumerable<long> zoneIds)
        {
            IEnumerable<CustomerCountry2> allCountries = new CustomerCountryManager().GetCustomerCountries(customerId);

            if (allCountries == null || allCountries.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("No Countries have ever been sold to Customer '{0}'", customerId));

            IEnumerable<CustomerCountry2> orderedCountries = allCountries.OrderBy(x => x.BED);

            var countryIds = new List<int>();
            var saleZoneManager = new SaleZoneManager();

            foreach (long zoneId in zoneIds)
            {
                int? countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);

                if (!countryId.HasValue)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Country of SaleZone '{0}' was not found", zoneId));

                if (!countryIds.Contains(countryId.Value))
                    countryIds.Add(countryId.Value);
            }

            foreach (CustomerCountry2 country in orderedCountries)
            {
                if (!countryIds.Contains(country.CountryId))
                    continue;

                List<CustomerCountry2> value;

                if (!_countryPeriodsByCountry.TryGetValue(country.CountryId, out value))
                {
                    value = new List<CustomerCountry2>();
                    _countryPeriodsByCountry.Add(country.CountryId, value);
                }

                value.Add(country);
            }
        }
        private void SetRates(int customerId, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            int sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
            IEnumerable<SaleRate> saleRates = new SaleRateManager().GetAllSaleRatesBySellingProductAndCustomer(zoneIds, sellingProductId, customerId, getNormalRates, getOtherRates);

            if (saleRates == null || saleRates.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();
            var salePriceListManager = new SalePriceListManager();

            foreach (SaleRate saleRate in saleRates.OrderBy(x => x.BED))
            {
                CustomerZoneRates rates;
                string zoneNameKey = getZoneNameKey(saleZoneManager.GetSaleZoneName(saleRate.ZoneId));

                if (!_ratesByZoneName.TryGetValue(zoneNameKey, out rates))
                {
                    rates = new CustomerZoneRates();
                    _ratesByZoneName.Add(zoneNameKey, rates);
                }

                SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRate.PriceListId);
                salePriceList.ThrowIfNull("salePriceList");

                var rateTypeKey = new RateTypeKey() { RateTypeId = saleRate.RateTypeId };
                Dictionary<RateTypeKey, List<SaleRate>> targetRates = (salePriceList.OwnerType == SalePriceListOwnerType.SellingProduct) ? rates.SellingProductZoneRatesByType : rates.CustomerZoneRatesByType;

                List<SaleRate> targetRatesOfType;

                if (!targetRates.TryGetValue(rateTypeKey, out targetRatesOfType))
                {
                    targetRatesOfType = new List<SaleRate>();
                    targetRates.Add(rateTypeKey, targetRatesOfType);
                }

                targetRatesOfType.Add(saleRate);
            }
        }
        private string getZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
