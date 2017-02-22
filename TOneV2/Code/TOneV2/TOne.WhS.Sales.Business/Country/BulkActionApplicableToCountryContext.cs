using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class BulkActionApplicableToCountryContext : IBulkActionApplicableToCountryContext
    {
        #region Fields / Constructors

        private Func<int, long, bool, SaleEntityZoneRate> _getSellingProudctZoneRate;

        private Func<int, int, long, bool, SaleEntityZoneRate> _getCustomerZoneRate;

        private Func<decimal?, decimal, DateTime> _getRateBED;

        public BulkActionApplicableToCountryContext(Func<int, long, bool, SaleEntityZoneRate> getSellingProudctZoneRate, Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate, Func<decimal?, decimal, DateTime> getRateBED)
        {
            _getSellingProudctZoneRate = getSellingProudctZoneRate;
            _getCustomerZoneRate = getCustomerZoneRate;
            _getRateBED = getRateBED;
        }

        #endregion

        public Country Country { get; set; }

        public int OwnerSellingNumberPlanId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

        public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate)
        {
            return _getSellingProudctZoneRate(sellingProductId, zoneId, getFutureRate);
        }

        public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId, bool getFutureRate)
        {
            return _getCustomerZoneRate(customerId, sellingProductId, zoneId, getFutureRate);
        }

        public DateTime GetRateBED(decimal? currentRateValue, decimal newRateValue)
        {
            return _getRateBED(currentRateValue, newRateValue);
        }
    }
}
