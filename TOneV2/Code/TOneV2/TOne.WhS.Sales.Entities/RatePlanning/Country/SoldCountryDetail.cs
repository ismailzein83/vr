using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class SoldCountryDetail
	{
		public SoldCountry Entity { get; set; }

		public bool IsSoldInFuture { get; set; }
	}
}
