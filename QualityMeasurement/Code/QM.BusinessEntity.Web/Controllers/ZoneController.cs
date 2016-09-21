using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace QM.BusinessEntity.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "Zone")]

    [JSONWithTypeAttribute]
    public class QMBE_ZoneController : BaseAPIController
    {

        [HttpGet]
        [Route("GetZoneSourceTemplates")]
        public IEnumerable<SourceZoneReaderConfig> GetZoneSourceTemplates()
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetZoneSourceTemplates();
        }


        [HttpGet]
        [Route("GetZonesInfo")]
        public IEnumerable<ZoneInfo> GetZonesInfo(string serializedFilter)
        {
            ZoneInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ZoneInfoFilter>(serializedFilter) : null;
            ZoneManager manager = new ZoneManager();
            return manager.GetZonesInfo(filter);
        }


        [HttpPost]
        [Route("GetFilteredZones")]
        public object GetFilteredZones(Vanrise.Entities.DataRetrievalInput<ZoneQuery> input)
        {
            ZoneManager manager = new ZoneManager();
            return GetWebResponse(input, manager.GetFilteredZones(input));
        }

    }
}