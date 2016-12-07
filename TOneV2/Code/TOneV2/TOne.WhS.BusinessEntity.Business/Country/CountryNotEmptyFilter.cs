using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
	public class CountryNotEmptyFilter : Vanrise.Entities.ICountryFilter
	{
		private SaleZoneManager _saleZoneManager = new SaleZoneManager();
		private IEnumerable<SaleZone> _sellingNumberPlanZones;

		public int CustomerId { get; set; }

		public DateTime EffectiveOn { get; set; }

		public bool IsExcluded(Vanrise.Entities.ICountryFilterContext context)
		{
			if (_sellingNumberPlanZones == null)
			{
				int sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(CustomerId, Entities.CarrierAccountType.Customer);
				_sellingNumberPlanZones = _saleZoneManager.GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
			}
			return !_saleZoneManager.AnySaleZoneEffectiveAfterExists(_sellingNumberPlanZones, context.Country.CountryId, EffectiveOn);
		}
	}
}
