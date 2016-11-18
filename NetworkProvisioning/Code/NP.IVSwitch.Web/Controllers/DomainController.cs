using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Domain")]
    [JSONWithTypeAttribute]
    public class DomainController : BaseAPIController
    {

        DomainManager _manager = new DomainManager();

        [HttpGet]
        [Route("GetDomainsInfo")]
        public IEnumerable<DomainInfo> GetDomainsInfo(string filter = null)
        {
            DomainFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<DomainFilter>(filter) : null;
            return _manager.GetDomainsInfo(deserializedFilter);
        }
    }
}