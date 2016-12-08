using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
	public class RatePlanContext : IRatePlanContext
	{
		private DateTime _retroactiveDate;

		public RatePlanContext()
		{
			int retroactiveDayOffset = new TOne.WhS.BusinessEntity.Business.ConfigManager().GetSaleAreaRetroactiveDayOffset();
			_retroactiveDate = DateTime.Now.Date.AddDays(-retroactiveDayOffset);
		}

		public SalePriceListOwnerType OwnerType { get; set; }
		public int OwnerId { get; set; }
		public DateTime EffectiveDate { get; set; }
		public SaleEntityZoneRateLocator RateLocator { get; set; }
		public DateTime RetroactiveDate
		{
			get
			{
				return _retroactiveDate;
			}
		}
	}

	public interface IRatePlanContext
	{
		SalePriceListOwnerType OwnerType { get; }
		int OwnerId { get; }
		DateTime EffectiveDate { get; }
		SaleEntityZoneRateLocator RateLocator { get; }
		DateTime RetroactiveDate { get; }
	}
}
