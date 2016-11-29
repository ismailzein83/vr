using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class SoldCountry
	{
		public int CountryId { get; set; }

		public string Name { get; set; }

		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}
}
