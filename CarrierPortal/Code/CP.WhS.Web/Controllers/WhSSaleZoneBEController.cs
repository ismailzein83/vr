using CP.WhS.Business;
using CP.WhS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.APIEntities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSSaleZoneBE")]
    [JSONWithTypeAttribute]
    public class WhSSaleZoneBEController : BaseAPIController
    {
        WhSSaleZoneBEManager _whSSaleZoneBEManager = new WhSSaleZoneBEManager();

        [HttpGet]
        [Route("GetRemoteSaleZonesInfo")]
        public IEnumerable<ClientSaleZoneInfo> GetRemoteSaleZonesInfo(string serializedFilter)
        {
            return _whSSaleZoneBEManager.GetRemoteSaleZonesInfo(Vanrise.Common.Serializer.Deserialize<ClientSaleZoneInfoFilter>(serializedFilter));
        }
    }
}