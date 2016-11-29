using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class CountryChanges
	{
		public IEnumerable<DraftNewCountry> NewCountries { get; set; }
		public DraftChangedCountries ChangedCountries { get; set; }
	}
}
