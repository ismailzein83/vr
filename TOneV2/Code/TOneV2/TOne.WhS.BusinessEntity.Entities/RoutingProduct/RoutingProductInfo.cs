using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class RoutingProductInfo
	{
		public int RoutingProductId { get; set; }

		public string Name { get; set; }

		public int SellingNumberPlanId { get; set; }

		public bool IsDefinedForAllZones { get; set; }
	}
}
