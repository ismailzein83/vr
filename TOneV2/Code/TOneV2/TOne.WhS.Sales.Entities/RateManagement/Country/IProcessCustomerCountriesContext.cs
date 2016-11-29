using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
	public interface IProcessCustomerCountriesContext
	{
		IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }

		IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }

		IEnumerable<ExistingCustomerCountry> ExistingCustomerCountries { get; set; }

		IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }

		IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }

		IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
	}
}
