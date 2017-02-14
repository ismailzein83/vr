using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ZoneRoutingProduct")]
    public class ZoneRoutingProductController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredZoneRoutingProducts")]
        public object GetFilteredZoneRoutingProducts(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductQuery> input)
        {
            ZoneRoutingProductManager manager = new ZoneRoutingProductManager();
            return GetWebResponse(input, manager.GetFilteredZoneRoutingProducts(input));
        }
        [HttpGet]
        [Route("GetPrimarySaleEntity")]
        public PrimarySaleEntity GetPrimarySaleEntity()
        {
            var manager = new ConfigManager();
            return manager.GetPrimarySaleEntity();
        }
    }
}