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
        private Dictionary<int, List<ProcessedCustomerSellingProduct>> _spAssignmentsBySP;
        private IEnumerable<int> _sellingProductIds;
        private Dictionary<int, List<CustomerCountry2>> _countryPeriodsByCountry;
        private Dictionary<string, CustomerZoneRates> _ratesByZone;
        #endregion

        #region Constructors
        public CustomerZoneRateHistoryReader(int customerId, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            SetSPAssignments(customerId);
            SetCountries(customerId, zoneIds);
            SetRates(customerId, zoneIds, getNormalRates, getOtherRates);
        }
        #endregion

        public Dictionary<int, List<ProcessedCustomerSellingProduct>> GetSellingProductAssignments(out IEnumerable<int> sellingProductIds)
        {
            sellingProductIds = _sellingProductIds;
            return _spAssignmentsBySP;
        }
        public IEnumerable<CustomerCountry2> GetCountryPeriods(int countryId)
        {
            return _countryPeriodsByCountry.GetRecord(countryId);
        }
        public CustomerZoneRates GetZoneRates(string zoneName)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
                throw new Vanrise.Entities.MissingArgumentValidationException("zoneName");

            string zoneNameKey = getZoneNameKey(zoneName);
            return _ratesByZone.GetRecord(zoneNameKey);
        }

        #region Private Methods
        private void SetSPAssignments(int customerId)
        {
            // CustomerSellingProductManager orders the list by BED
            IEnumerable<ProcessedCustomerSellingProduct> spAssignments = new CustomerSellingProductManager().GetProcessedCustomerSellingProducts(customerId);

            if (spAssignments == null || spAssignments.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' has never been assigned to a SellingProduct", customerId));

            _spAssignmentsBySP = new Dictionary<int, List<ProcessedCustomerSellingProduct>>();
            var distinctSPIds = new List<int>();

            foreach (ProcessedCustomerSellingProduct spAssignment in spAssignments.OrderBy(x => x.BED))
            {
                List<ProcessedCustomerSellingProduct> value;

                if (!_spAssignmentsBySP.TryGetValue(spAssignment.SellingProductId, out value))
                {
                    value = new List<ProcessedCustomerSellingProduct>();
                    _spAssignmentsBySP.Add(spAssignment.SellingProductId, value);
                }

                if (!distinctSPIds.Contains(spAssignment.SellingProductId))
                    distinctSPIds.Add(spAssignment.SellingProductId);

                value.Add(spAssignment);
            }

            _sellingProductIds = distinctSPIds;
        }
        private void SetCountries(int customerId, IEnumerable<long> zoneIds)
        {
            IEnumerable<CustomerCountry2> allCountries = new CustomerCountryManager().GetCustomerCountries(customerId);

            if (allCountries == null || allCountries.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("No Countries have ever been sold to Customer '{0}'", customerId));

            _countryPeriodsByCountry = new Dictionary<int, List<CustomerCountry2>>();
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
            _ratesByZone = new Dictionary<string, CustomerZoneRates>();

            IEnumerable<SaleRate> saleRates = new SaleRateManager().GetAllSaleRatesBySellingProductsAndCustomer(zoneIds, _sellingProductIds, customerId, getNormalRates, getOtherRates);

            if (saleRates == null || saleRates.Count() == 0)
                return;

            var saleZoneManager = new SaleZoneManager();
            var salePriceListManager = new SalePriceListManager();

            foreach (SaleRate saleRate in saleRates.OrderBy(x => x.BED))
            {
                string zoneName = saleZoneManager.GetSaleZoneName(saleRate.ZoneId);

                if (string.IsNullOrWhiteSpace(zoneName))
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of SaleZone '{0}' was not found", saleRate.ZoneId));

                CustomerZoneRates rates;
                string zoneNameKey = getZoneNameKey(zoneName);

                if (!_ratesByZone.TryGetValue(zoneNameKey, out rates))
                {
                    rates = new CustomerZoneRates();
                    _ratesByZone.Add(zoneNameKey, rates);
                }

                SalePriceList salePriceList = salePriceListManager.GetPriceList(saleRate.PriceListId);

                if (salePriceList == null)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SalePriceList '{0}' of SaleRate '{1}' of SaleZone '{2}' was not found", saleRate.PriceListId, saleRate.SaleRateId, saleRate.ZoneId));

                var rateTypeKey = new RateTypeKey() { RateTypeId = saleRate.RateTypeId };

                if (salePriceList.OwnerType == SalePriceListOwnerType.Customer) AddCustomerZoneRate(rates, rateTypeKey, saleRate);
                else AddSellingProductZoneRate(salePriceList.OwnerId, rates, rateTypeKey, saleRate);
            }
        }
        private void AddCustomerZoneRate(CustomerZoneRates rates, RateTypeKey rateTypeKey, SaleRate saleRate)
        {
            List<SaleRate> ratesByType;

            if (!rates.CustomerZoneRatesByType.TryGetValue(rateTypeKey, out ratesByType))
            {
                ratesByType = new List<SaleRate>();
                rates.CustomerZoneRatesByType.Add(rateTypeKey, ratesByType);
            }

            ratesByType.Add(saleRate);
        }
        private void AddSellingProductZoneRate(int sellingProductId, CustomerZoneRates rates, RateTypeKey rateTypeKey, SaleRate saleRate)
        {
            // Add to rates.SellingProductZoneRates

            Dictionary<int, List<SaleRate>> spZoneRatesByType;

            if (!rates.SellingProductZoneRatesByType.TryGetValue(rateTypeKey, out spZoneRatesByType))
            {
                spZoneRatesByType = new Dictionary<int, List<SaleRate>>();
                rates.SellingProductZoneRatesByType.Add(rateTypeKey, spZoneRatesByType);
            }

            List<SaleRate> spZoneRates;

            if (!spZoneRatesByType.TryGetValue(sellingProductId, out spZoneRates))
            {
                spZoneRates = new List<SaleRate>();
                spZoneRatesByType.Add(sellingProductId, spZoneRates);
            }

            spZoneRates.Add(saleRate);
        }
        /// <param name="zoneName">Assumed to be != null and != whitespace</param>
        private string getZoneNameKey(string zoneName)
        {
            return zoneName.Trim().ToLower();
        }
        #endregion
    }
}
