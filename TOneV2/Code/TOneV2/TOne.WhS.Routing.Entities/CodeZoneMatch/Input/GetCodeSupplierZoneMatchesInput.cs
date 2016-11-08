using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
	public class GetCodeSupplierZoneMatchesInput
	{
		public IEnumerable<int> SaleZoneIds { get; set; }

		public IEnumerable<int> SupplierIds { get; set; }
	}
}
