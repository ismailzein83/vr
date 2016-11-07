using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Extensions.TelesSwitch
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelestSwitch")]
    public class TelestSwitchController : BaseAPIController
    {
        TelestSwitchManager _manager = new TelestSwitchManager();
        [HttpGet]
        [Route("GetDomains")]
        public object GetDomains()
        {
            return _manager.GetDomains();
        }
        [HttpGet]
        [Route("GetGateWays")]
        public object GetGateWays(string domain)
        {
            return _manager.GetGateWays(domain);
        }
    }
}
