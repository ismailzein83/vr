using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public class NewCustomerCountryPreview
	{
		public int CountryId { get; set; }
        public int CustomerId { get; set; }
		public DateTime BED { get; set; }
		public DateTime? EED { get; set; }
	}

	public class NewCustomerCountryPreviewDetail
	{
		public NewCustomerCountryPreview Entity { get; set; }
		public string CountryName { get; set; }
        public string CustomerName { get; set; }
	}
}
