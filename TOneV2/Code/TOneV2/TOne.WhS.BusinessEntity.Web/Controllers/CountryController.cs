using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Country")]
    public class WhSBE_CountryController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCountries")]
        public object GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.CountryQuery> input)
        {
            Vanrise.Common.Business.CountryManager manager = new Vanrise.Common.Business.CountryManager();
            return GetWebResponse(input, manager.GetFilteredCountries(input));
        }

        [HttpGet]
        [Route("GetAllCountries")]
        public IEnumerable<Vanrise.Entities.Country> GetAllCountries()
        {
            Vanrise.Common.Business.CountryManager manager = new Vanrise.Common.Business.CountryManager();
            return manager.GetAllCountries();
        }
        [HttpGet]
        [Route("GetCountry")]
        public Vanrise.Entities.Country GetCountry(int countryId)
        {
            Vanrise.Common.Business.CountryManager manager = new Vanrise.Common.Business.CountryManager();
            return manager.GetCountry(countryId);
        }

        [HttpPost]
        [Route("AddCountry")]
        public Vanrise.Entities.InsertOperationOutput<Vanrise.Entities.Country> AddCountry(Vanrise.Entities.Country country)
        {
            Vanrise.Common.Business.CountryManager manager = new Vanrise.Common.Business.CountryManager();
            return manager.AddCountry(country);
        }
        [HttpPost]
        [Route("UpdateCountry")]
        public Vanrise.Entities.UpdateOperationOutput<Vanrise.Entities.Country> UpdateCountry(Vanrise.Entities.Country country)
        {
            Vanrise.Common.Business.CountryManager manager = new Vanrise.Common.Business.CountryManager();
            return manager.UpdateCountry(country);
        }
       
    }
}