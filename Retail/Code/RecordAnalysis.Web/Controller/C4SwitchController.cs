using RecordAnalysis.Entities;
using RecordAnalysis.Business;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace RecordAnalysis.Web.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "C4Switch")]
    public class C4SwitchController : BaseAPIController
    {
        [HttpGet]
        [Route("GetC4SwitchTemplates")]
        public IEnumerable<C4SwitchSettingsConfig> GetC4SwitchTemplates()
        {
            C4SwitchManager manager = new C4SwitchManager();
            return manager.GetC4SwitchTemplates();
        }
    }
}