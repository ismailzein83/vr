using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Country")]
    public class WhSBE_CountryController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCountries")]
        public object GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            CountryManager manager = new CountryManager();
            return GetWebResponse(input, manager.GetFilteredCountries(input));
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
        public TOne.Entities.InsertOperationOutput<Country> AddCountry(Country country)
        {
            CountryManager manager = new CountryManager();
            return manager.AddCountry(country);
        }
        [HttpPost]
        [Route("UpdateCountry")]
        public TOne.Entities.UpdateOperationOutput<Country> UpdateCountry(Country country)
        {
            CountryManager manager = new CountryManager();
            return manager.UpdateCountry(country);
        }
       
    }
}