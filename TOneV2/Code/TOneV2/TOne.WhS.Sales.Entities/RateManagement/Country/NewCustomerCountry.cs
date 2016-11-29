using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class NewCustomerCountry
	{
		public long CustomerCountryId { get; set; }

		public int CustomerId { get; set; }

		public int CountryId { get; set; }
		
		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}
}
