using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
	public class SaleZoneCountrySoldToCustomerFilter : ISaleZoneFilter
	{
		public int CustomerId { get; set; }

		public DateTime? EffectiveOn { get; set; }

		public bool IsEffectiveInFuture { get; set; }

		public bool IsExcluded(ISaleZoneFilterContext context)
		{
			if (context.CustomData == null)
			{
				context.CustomData = (object)UtilitiesManager.GetDatesByCountry(CustomerId, EffectiveOn, IsEffectiveInFuture);
			}
			var datesByCountry = context.CustomData as Dictionary<int, DateTime>;
			return (!datesByCountry.ContainsKey(context.SaleZone.CountryId));
		}
	}
}
