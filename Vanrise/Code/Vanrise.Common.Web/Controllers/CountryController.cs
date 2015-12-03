using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Country")]
    public class VRCommon_CountryController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCountries")]
        public object GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            CountryManager manager = new CountryManager();
            return GetWebResponse(input, manager.GetFilteredCountries(input));
        }

        [HttpGet]
        [Route("GetCountriesInfo")]
        public IEnumerable<CountryInfo> GetCountriesInfo()
        {
            CountryManager manager = new CountryManager();
            return manager.GeCountriesInfo();
        }
        [HttpGet]
        [Route("GetCountry")]
        public Country GetCountry(int countryId)
        {
            CountryManager manager = new CountryManager();
            return manager.GetCountry(countryId);
        }

        [HttpPost]
        [Route("AddCountry")]
        public Vanrise.Entities.InsertOperationOutput<CountryDetail> AddCountry(Country country)
        {
            CountryManager manager = new CountryManager();
            return manager.AddCountry(country);
        }
        [HttpPost]
        [Route("UpdateCountry")]
        public Vanrise.Entities.UpdateOperationOutput<CountryDetail> UpdateCountry(Country country)
        {
            CountryManager manager = new CountryManager();
            return manager.UpdateCountry(country);
        }
       
    }
}