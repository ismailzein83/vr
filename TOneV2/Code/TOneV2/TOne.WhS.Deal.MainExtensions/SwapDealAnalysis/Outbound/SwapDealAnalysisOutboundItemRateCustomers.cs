using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
    public class SwapDealAnalysisOutboundItemRateCustomers : SwapDealAnalysisOutboundItemRateCalcMethod
    {
        public CarrierFilterType CustomerFilterType { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public ZoneRateEvaluationType RateEvaluationType { get; set; }

        public override decimal? Execute(ISwapDealAnalysisOutboundRateCalcMethodContext context)
        {
            var codeZoneMatchManager = new CodeZoneMatchManager();
            IEnumerable<SaleZoneDefintion> codeSaleZoneMatches = codeZoneMatchManager.GetSaleZonesMatchedToSupplierZones(context.SupplierZoneIds);

            if (codeSaleZoneMatches == null || codeSaleZoneMatches.Count() == 0)
                return null;

            ZonesBySellingNumberPlan data = GetZonesBySellingNumberPlan(codeSaleZoneMatches);
            IEnumerable<int> customerIds = GetCustomerIdsOfSpecificCountries(context.CountryIds);

            if (customerIds == null)
                return null;

            var rates = new List<decimal>();
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(DateTime.Now));
            var customerSellingProductManager = new CustomerSellingProductManager();
            var carrierAccountManager = new CarrierAccountManager();

            foreach (int customerId in customerIds)
            {
                int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, DateTime.Now, false);
                if (!sellingProductId.HasValue)
                    continue;

                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(customerId);

                if (!data.TryGetValue(sellingNumberPlanId, out List<long> saleZoneIds))
                    continue;

                foreach (long saleZoneId in saleZoneIds)
                {
                    SaleEntityZoneRate rate = rateLocator.GetCustomerZoneRate(customerId, sellingProductId.Value, saleZoneId);
                    if (rate != null && rate.Rate != null)
                        rates.Add(rate.Rate.Rate);
                }
            }

            return GetCalculatedRate(rates);
        }

        #region Private Members

        private class ZonesBySellingNumberPlan : Dictionary<int, List<long>>
        {

        }

        private ZonesBySellingNumberPlan GetZonesBySellingNumberPlan(IEnumerable<SaleZoneDefintion> codeSaleZoneMatches)
        {
            var zonesBySellingNumberPlan = new ZonesBySellingNumberPlan();

            foreach (SaleZoneDefintion codeSaleZoneMatch in codeSaleZoneMatches)
            {
                List<long> saleZoneIds;
                if (!zonesBySellingNumberPlan.TryGetValue(codeSaleZoneMatch.SellingNumberPlanId, out saleZoneIds))
                {
                    saleZoneIds = new List<long>();
                    zonesBySellingNumberPlan.Add(codeSaleZoneMatch.SellingNumberPlanId, saleZoneIds);
                }
                if (!saleZoneIds.Contains(codeSaleZoneMatch.SaleZoneId))
                    saleZoneIds.Add(codeSaleZoneMatch.SaleZoneId);
            }

            return zonesBySellingNumberPlan;
        }

        private IEnumerable<int> GetCustomerIdsOfSpecificCountries(List<int> countryIds)
        {
            var carrierAccountManager = new CarrierAccountManager();
            IEnumerable<CarrierAccount> allCustomers = carrierAccountManager.GetAllCustomers();

            if (allCustomers == null)
                throw new NullReferenceException("allCustomers");

            Func<CarrierAccount, bool> filterFunc = null;
            switch (CustomerFilterType)
            {
                case CarrierFilterType.Specific:
                    RequireCustomerIds();
                    filterFunc = (carrierAccount) => { return CustomerIds.Contains(carrierAccount.CarrierAccountId); };
                    break;
                case CarrierFilterType.AllExcept:
                    RequireCustomerIds();
                    filterFunc = (carrierAccount) => { return !CustomerIds.Contains(carrierAccount.CarrierAccountId); };
                    break;
            }
            IEnumerable<CarrierAccount> filteredCustomers = (filterFunc != null) ? allCustomers.FindAllRecords(filterFunc) : allCustomers;
            if (filteredCustomers == null)
                throw new NullReferenceException("filteredCustomers");

            var customerCountryManager = new CustomerCountryManager();
            IEnumerable<int> customerIds = customerCountryManager.GetCustomerIdsOfSpecificCountries(filteredCustomers, countryIds);

            return customerIds.Distinct();
        }

        private void RequireCustomerIds()
        {
            if (CustomerIds == null)
                throw new NullReferenceException("CustomerIds");
        }

        private decimal? GetCalculatedRate(IEnumerable<decimal> rates)
        {
            if (rates.Count() == 0)
                return null;
            switch (RateEvaluationType)
            {
                case ZoneRateEvaluationType.AverageRate:
                    return rates.Average();
                case ZoneRateEvaluationType.MaximumRate:
                    return rates.Max();
                case ZoneRateEvaluationType.BasedOnTraffic:
                    throw new NotImplementedException("ZoneRateEvaluationType.BasedOnTraffic");
            }
            throw new Exception("RateEvaluationType");
        }

        #endregion
    }
}
