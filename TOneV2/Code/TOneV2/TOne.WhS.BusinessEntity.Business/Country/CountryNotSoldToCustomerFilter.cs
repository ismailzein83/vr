using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Business
{
	public class CountryNotSoldToCustomerFilter : Vanrise.Entities.ICountryFilter
	{
		private CustomerCountryManager _customerCountryManager = new CustomerCountryManager();

		public int CustomerId { get; set; }

		public DateTime EffectiveOn { get; set; }

		public bool IsExcluded(Vanrise.Entities.ICountryFilterContext context)
		{
			return (_customerCountryManager.IsCountrySoldToCustomer(CustomerId, context.Country.CountryId, EffectiveOn));
		}
	}
}
