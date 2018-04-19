using System;
using System.Web.Http;
using System.Collections.Generic;
using Vanrise.Web.Base;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchCommunication")]
    [JSONWithTypeAttribute]
    public class SwitchCommunicationController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSwitchCommunicationTemplates")]
        public IEnumerable<SwitchCommunicationConfig> GetSwitchCommunicationTemplates()
        {
            SwitchCommunicationManager manager = new SwitchCommunicationManager();
            return manager.GetSwitchCommunicationTemplates();
        }
    }
}