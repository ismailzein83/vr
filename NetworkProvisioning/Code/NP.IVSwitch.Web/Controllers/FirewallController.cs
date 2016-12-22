using System.Web.Http;
using NP.IVSwitch.Entities;
using Vanrise.Web.Base;
using NP.IVSwitch.Business;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Firewall")]
    [JSONWithTypeAttribute]
    public class FirewallController : BaseAPIController
    {
        FirewallManager _manager = new FirewallManager();
        [HttpPost]
        [Route("GetFilteredFirewalls")]
        public object GetFilteredFirewalls(Vanrise.Entities.DataRetrievalInput<FirewallQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredFirewalls(input));
        }
    }
}