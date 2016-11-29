using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
	public class PriceListCustomerCountryManager
	{
		public void ProcessCustomerCountries(IProcessCustomerCountriesContext context)
		{
			List<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries;
			Process(context.CustomerCountriesToAdd, context.CustomerCountriesToChange, context.ExistingCustomerCountries, out explicitlyChangedExistingCustomerCountries);
			context.NewCustomerCountries = context.CustomerCountriesToAdd.MapRecords(x => x.NewCustomerCountry);
			context.ChangedCustomerCountries = context.ExistingCustomerCountries.MapRecords(x => x.ChangedCustomerCountry, x => x.ChangedCustomerCountry != null);
			context.ExplicitlyChangedExistingCustomerCountries = explicitlyChangedExistingCustomerCountries;
		}

		private void Process(IEnumerable<CustomerCountryToAdd> countriesToAdd, IEnumerable<CustomerCountryToChange> countriesToChange, IEnumerable<ExistingCustomerCountry> existingCountries, out List<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries)
		{
			Dictionary<int, List<ExistingCustomerCountry>> structuredCountries = StructureExistingCountries(existingCountries);

			foreach (CustomerCountryToAdd countryToAdd in countriesToAdd)
			{
				List<ExistingCustomerCountry> matchedExistingCountries;

				if (structuredCountries.TryGetValue(countryToAdd.CountryId, out matchedExistingCountries))
				{
					CloseOverlappedExistingCountries(countryToAdd, matchedExistingCountries);
				}

				ProcessCountryToAdd(countryToAdd);
			}

			explicitlyChangedExistingCustomerCountries = new List<ExistingCustomerCountry>();

			foreach (CustomerCountryToChange countryToChange in countriesToChange)
			{
				List<ExistingCustomerCountry> matchedExistingCountries = structuredCountries.GetRecord(countryToChange.CountryId);
				if (matchedExistingCountries == null)
					throw new NullReferenceException("matchedExistingCountries");
				CloseExistingCountries(countryToChange, matchedExistingCountries, explicitlyChangedExistingCustomerCountries);
			}
		}

		private Dictionary<int, List<ExistingCustomerCountry>> StructureExistingCountries(IEnumerable<ExistingCustomerCountry> existingCountries)
		{
			var structuredCountries = new Dictionary<int, List<ExistingCustomerCountry>>();

			foreach (ExistingCustomerCountry existingCountry in existingCountries)
			{
				List<ExistingCustomerCountry> countries;
				if (!structuredCountries.TryGetValue(existingCountry.CustomerCountryEntity.CountryId, out countries))
				{
					countries = new List<ExistingCustomerCountry>();
					structuredCountries.Add(existingCountry.CustomerCountryEntity.CountryId, countries);
				}
				countries.Add(existingCountry);
			}

			return structuredCountries;
		}

		private void CloseOverlappedExistingCountries(CustomerCountryToAdd countryToAdd, IEnumerable<ExistingCustomerCountry> matchedExistingCountries)
		{
			foreach (ExistingCustomerCountry existingCountry in matchedExistingCountries)
			{
				if (existingCountry.IsOverlappedWith(countryToAdd))
				{
					existingCountry.ChangedCustomerCountry = new ChangedCustomerCountry()
					{
						CustomerCountryId = existingCountry.CustomerCountryEntity.CustomerCountryId,
						EED = Vanrise.Common.Utilities.Max(existingCountry.BED, countryToAdd.BED)
					};
					countryToAdd.ChangedExistingCustomerCountries.Add(existingCountry);
				}
			}
		}

		private void ProcessCountryToAdd(CustomerCountryToAdd countryToAdd)
		{
			countryToAdd.NewCustomerCountry = new NewCustomerCountry()
			{
				CustomerId = countryToAdd.CustomerId,
				CountryId = countryToAdd.CountryId,
				BED = countryToAdd.BED,
				EED = countryToAdd.EED
			};
		}

		private void CloseExistingCountries(CustomerCountryToChange countryToChange, IEnumerable<ExistingCustomerCountry> matchedExistingCountries, List<ExistingCustomerCountry> explicitlyChangedCountries)
		{
			foreach (ExistingCustomerCountry existingCountry in matchedExistingCountries)
			{
				if (existingCountry.EED.VRGreaterThan(countryToChange.CloseEffectiveDate))
				{
					existingCountry.ChangedCustomerCountry = new ChangedCustomerCountry()
					{
						CustomerCountryId = existingCountry.CustomerCountryEntity.CustomerCountryId,
						EED = Vanrise.Common.Utilities.Max(existingCountry.BED, countryToChange.CloseEffectiveDate)
					};
					countryToChange.ChangedExistingCustomerCountries.Add(existingCountry);
					explicitlyChangedCountries.Add(existingCountry);
				}
			}
		}
	}
}
