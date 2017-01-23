using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class RoutingProductInfoFilter
	{
		public int? ExcludedRoutingProductId { get; set; }

		public SalePriceListOwnerType? AssignableToOwnerType { get; set; }

		public int? AssignableToOwnerId { get; set; }

		public long? AssignableToZoneId { get; set; }

		public int? SellingNumberPlanId { get; set; }
	}
}
