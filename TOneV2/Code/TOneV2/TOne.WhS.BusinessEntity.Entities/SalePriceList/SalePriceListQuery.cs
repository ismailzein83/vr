using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class SalePriceListQuery
	{
		public SalePriceListOwnerType? OwnerType { get; set; }
		public IEnumerable<int> OwnerIds { get; set; }
	}
}
