using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Zone")]
    [JSONWithTypeAttribute]
    public class ZoneController : BaseAPIController
    {
        [HttpGet]
        [Route("GetZonesByCountryId")]
        public List<Zone> GetZonesByCountryId(string serializedFilter)
        {
            ZoneFilter filter = !string.IsNullOrEmpty(serializedFilter) ? Vanrise.Common.Serializer.Deserialize<ZoneFilter>(serializedFilter) : null;
            ZoneManager manager = new ZoneManager();
            return manager.GetZonesByCountryId(filter != null ? filter.CountryId : 0);
        }
    }
}