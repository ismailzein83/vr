using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using Vanrise.Web.Base;
using Vanrise.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
    [JSONWithTypeAttribute]
    public class WhS_RouteSync_SwitchController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAllSwitches")]
        public List<SwitchInfo> GetAllSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetAllSwitches();
        }
    }
}