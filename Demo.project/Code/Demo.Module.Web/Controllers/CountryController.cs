using System.Web;
using Vanrise.Entities;
using Vanrise.Web.Base;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Web.Controllers
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Country")]
	[JSONWithTypeAttribute]
	public class CountryController : BaseAPIController
	{
		CountryManager countryManager = new CountryManager();

		[HttpPost]	
		[Route("GetFilteredCountries")]
		public object GetFilteredCountries(DataRetrievalInput<Entities.CountryQuery> input)
		{
			return GetWebResponse(input, countryManager.GetFilteredCountries(input));
		}

		[HttpGet]
		[Route("GetCountriesInfo")]
		public IEnumerable<Entities.CountryInfo> GetCountriesInfo(string filter = null)
		{
			CountryInfoFilter countryInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<CountryInfoFilter>(filter) : null;
			return countryManager.GetCountryInfo(countryInfoFilter);
		}

		[HttpGet]
		[Route("GetCountryById")]
		public Entities.Country GetCountryById(int countryId)
		{
			return countryManager.GetCountryById(countryId);
		}

		[HttpPost]	
		[Route("AddCountry")]
		public InsertOperationOutput<CountryDetails> AddCountry(Entities.Country country)
		{
			return countryManager.AddCountry(country);
		}

		[HttpPost]
		[Route("UpdateCountry")]
		public UpdateOperationOutput<CountryDetails> UpdateCountry(Entities.Country country)
		{
			return countryManager.UpdateCountry(country);
		}
	}
}