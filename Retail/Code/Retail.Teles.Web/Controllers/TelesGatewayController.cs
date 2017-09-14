using Retail.Teles.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Teles.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesGateway")]
    public class TelesGatewayController : BaseAPIController
    {
        TelesGatewayManager _manager = new TelesGatewayManager();

        [HttpGet]
        [Route("GetGatewaysInfo")]
        public IEnumerable<TelesGatewayInfo> GetGatewaysInfo(Guid vrConnectionId, string siteId, string serializedFilter = null)
        {
            TelesGatewayFilter filter = Vanrise.Common.Serializer.Deserialize<TelesGatewayFilter>(serializedFilter);
            return _manager.GetGatewaysInfo(vrConnectionId, siteId, filter);
        }
    }
}