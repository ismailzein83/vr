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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchDashboard")]
    [JSONWithTypeAttribute]
    public class SwitchDashboardController:BaseAPIController
    {
        SwitchDashboardManager _manager = new SwitchDashboardManager();

        [HttpGet]
        [Route("GetSwitchDashboardManagerResult")]
        public LiveDashboardResult GetSwitchDashboardManagerResult()
        {
            return _manager.GetSwitchDashboardManagerResult();
        }     
    }
}