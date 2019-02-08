using System.Collections.Generic;
using System.Web.Http;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;
using Vanrise.Web.Base;

namespace Vanrise.MobileNetwork.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "MobileCountry")]
    public class MobileCountryController : Vanrise.Web.Base.BaseAPIController
    {
        MobileCountryManager mobileCountryManager = new MobileCountryManager();

        [HttpGet]
        [Route("GetMobileCountriesInfo")]
        public IEnumerable<MobileCountryInfo> GetMobileCountriesInfo(string serializedFilter = null)
        {
            var deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<MobileCountryInfoFilter>(serializedFilter) : null;
            return mobileCountryManager.GetMobileCountriesInfo(deserializedFilter);
        }
    }
}