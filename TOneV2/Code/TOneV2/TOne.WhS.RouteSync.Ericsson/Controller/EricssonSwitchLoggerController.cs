using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Ericsson.Business;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Ericsson.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "EricssonSwitchLogger")]
    [JSONWithTypeAttribute]
    public class EricssonSwitchLoggerController : BaseAPIController
    {
        SwitchLoggerManager _manager = new SwitchLoggerManager();

        [HttpGet]
        [Route("GetSwitchLoggerTemplates")]
        public IEnumerable<SwitchLoggerConfig> GetSwitchLoggerTemplates()
        {
            return _manager.GetSwitchLoggerTemplates();
        }
    }
}