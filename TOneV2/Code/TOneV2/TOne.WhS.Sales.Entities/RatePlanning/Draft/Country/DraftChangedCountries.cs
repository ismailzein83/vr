using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class DraftChangedCountries
	{
		public IEnumerable<int> CountryIds { get; set; }
		public DateTime EED { get; set; }
	}
}
