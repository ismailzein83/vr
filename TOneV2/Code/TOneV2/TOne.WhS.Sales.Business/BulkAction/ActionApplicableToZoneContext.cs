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
		private Func<int, long, SaleEntityZoneRate> _getSellingProductZoneRate;

		private Func<int, int, long, SaleEntityZoneRate> _getCustomerZoneRate;

		public ActionApplicableToZoneContext(Func<int, long, SaleEntityZoneRate> getSellingProductZoneRate, Func<int, int, long, SaleEntityZoneRate> getCustomerZoneRate)
		{
			_getSellingProductZoneRate = getSellingProductZoneRate;
			_getCustomerZoneRate = getCustomerZoneRate;
		}

		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public SaleZone SaleZone { get; set; }

		public ZoneChanges ZoneDraft { get; set; }

		public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId)
		{
			return _getSellingProductZoneRate(sellingProductId, zoneId);
		}

		public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId)
		{
			return _getCustomerZoneRate(customerId, sellingProductId, zoneId);
		}
	}
}
