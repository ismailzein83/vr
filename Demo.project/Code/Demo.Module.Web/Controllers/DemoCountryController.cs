using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DemoCountry")]
    [JSONWithTypeAttribute]
    public class DemoCountryController : BaseAPIController
    {
        DemoCountryManager demoCountryManager = new DemoCountryManager();
       

        [HttpGet]
        [Route("GetDemoCountryById")]
        public DemoCountry GetCountryById(int demoCountryId)
        {
            return demoCountryManager.GetDemoCountryById(demoCountryId);
        }

        [HttpGet]
        [Route("GetDemoCountriesInfo")]
        public IEnumerable<DemoCountryInfo> GetDemoCountriesInfo(string filter = null)
        {
            DemoCountryInfoFilter DemoCountryInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DemoCountryInfoFilter>(filter) : null;
            return demoCountryManager.GetDemoCountriesInfo(DemoCountryInfoFilter);
        }
        

    }
}