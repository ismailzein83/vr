using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class SoldCountryQuery
	{
		public int CustomerId { get; set; }

		public DateTime EffectiveOn { get; set; }
	}
}
