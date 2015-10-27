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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeGroup")]
    public class WhSBE_CodeGroupController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredCodeGroups")]
        public object GetFilteredCodeGroups(Vanrise.Entities.DataRetrievalInput<CodeGroupQuery> input)
        {
            CodeGroupManager manager = new CodeGroupManager();
            return GetWebResponse(input, manager.GetFilteredCodeGroups(input));
        }

        //[HttpGet]
        //[Route("GetAllCountries")]
        //public IEnumerable<Country> GetAllCountries()
        //{
        //    CountryManager manager = new CountryManager();
        //    return manager.GetAllCountries();
        //}
        //[HttpGet]
        //[Route("GetCountry")]
        //public Country GetCountry(int countryId)
        //{
        //    CountryManager manager = new CountryManager();
        //    return manager.GetCountry(countryId);
        //}

        //[HttpPost]
        //[Route("AddCountry")]
        //public TOne.Entities.InsertOperationOutput<Country> AddCountry(Country country)
        //{
        //    CountryManager manager = new CountryManager();
        //    return manager.AddCountry(country);
        //}
        //[HttpPost]
        //[Route("UpdateCountry")]
        //public TOne.Entities.UpdateOperationOutput<Country> UpdateCountry(Country country)
        //{
        //    CountryManager manager = new CountryManager();
        //    return manager.UpdateCountry(country);
        //}
       
    }
}