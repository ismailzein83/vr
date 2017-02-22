using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
	public class ActionApplicableToZoneContext : IActionApplicableToZoneContext
	{
		private Func<int, long, bool, SaleEntityZoneRate> _getSellingProductZoneRate;

		private Func<int, int, long, bool, SaleEntityZoneRate> _getCustomerZoneRate;

        private Func<decimal?, decimal, DateTime> _getRateBED;

		public ActionApplicableToZoneContext(Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate, Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate, Func<decimal?, decimal, DateTime> getRateBED)
		{
			_getSellingProductZoneRate = getSellingProductZoneRate;
			_getCustomerZoneRate = getCustomerZoneRate;
            _getRateBED = getRateBED;
		}

		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public SaleZone SaleZone { get; set; }

		public ZoneChanges ZoneDraft { get; set; }

		public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate)
		{
			return _getSellingProductZoneRate(sellingProductId, zoneId, getFutureRate);
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
