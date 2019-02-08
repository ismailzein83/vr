using System.Collections.Generic;
using System.Web.Http;
using Vanrise.MobileNetwork.Business;
using Vanrise.MobileNetwork.Entities;
using Vanrise.Web.Base;

namespace Vanrise.MobileNetwork.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "MobileNetwork")]
    public class MobileNetworkController : Vanrise.Web.Base.BaseAPIController
    {
        MobileNetworkManager mobileNetworkManager = new MobileNetworkManager();

        [HttpGet]
        [Route("GetMobileNetworksInfo")]
        public IEnumerable<MobileNetworkInfo> GetMobileNetworksInfo(string serializedFilter = null)
        {
            var deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<MobileNetworkInfoFilter>(serializedFilter) : null;
            return mobileNetworkManager.GetMobileNetworksInfo(deserializedFilter);
        }
    }
}