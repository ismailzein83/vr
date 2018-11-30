using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRRestCountry")]
    public class VRRestCountryController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRemoteCountriesInfo")]
        public IEnumerable<CountryInfo> GetRemoteCountriesInfo(Guid connectionId,string filter= null)
        {
            VRRestCountryManager manager = new VRRestCountryManager();
            return manager.GetRemoteCountriesInfo(connectionId, filter);
        }
    }
}