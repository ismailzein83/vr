using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
	public class ProcessCustomerCountriesContext : IProcessCustomerCountriesContext
	{
		public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }

		public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }

		public IEnumerable<ExistingCustomerCountry> ExistingCustomerCountries { get; set; }

		public IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }

		public IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }

		public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
	}
}
