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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesDomain")]
    public class TelesDomainController : BaseAPIController
    {
        TelesDomainManager _manager = new TelesDomainManager();

        [HttpGet]
        [Route("GetDomainsInfo")]
        public IEnumerable<TelesDomainInfo> GetDomainsInfo(Guid vrConnectionId, string serializedFilter = null)
        {
            TelesDomainFilter filter = Vanrise.Common.Serializer.Deserialize<TelesDomainFilter>(serializedFilter);
            return _manager.GetDomainsInfo(vrConnectionId, filter);
        }
    }
}