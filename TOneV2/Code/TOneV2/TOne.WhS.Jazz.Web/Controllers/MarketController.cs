using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Market")]
    public class MarketController : BaseAPIController
    {
        MarketManager _manager = new MarketManager();

        [HttpGet]
        [Route("GetMarketsInfo")]
        public IEnumerable<MarketDetail> GetMarketsInfo(string filter=null)
        {
            MarketInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<MarketInfoFilter>(filter) : null;
            return _manager.GetMarketsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllMarkets")]
        public IEnumerable<Market> GetAllMarkets()
        {
            return _manager.GetAllMarkets();
        }
    }
}