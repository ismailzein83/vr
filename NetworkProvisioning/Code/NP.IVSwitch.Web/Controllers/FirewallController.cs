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
        [HttpGet]
        [Route("GetFirewall")]
        public Firewall GetFirewall(int firewallId)
        {
            return _manager.GetFirewall(firewallId);
        }

        [HttpPost]
        [Route("AddFirewall")]
        public Vanrise.Entities.InsertOperationOutput<FirewallDetail> AddFirewall(Firewall firewallItem)
        {
            return _manager.AddFirewall(firewallItem);
        }

        [HttpPost]
        [Route("UpdateFirewall")]
        public Vanrise.Entities.UpdateOperationOutput<FirewallDetail> UpdateFirewall(Firewall firewallItem)
        {
            return _manager.UpdateFirewall(firewallItem);
        }
    }
}