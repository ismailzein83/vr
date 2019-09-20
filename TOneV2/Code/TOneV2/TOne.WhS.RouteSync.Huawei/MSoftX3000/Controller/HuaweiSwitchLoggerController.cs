using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Huawei.Business;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Huawei.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "HuaweiSwitchLogger")]
    [JSONWithTypeAttribute]
    public class HuaweiSwitchLoggerController : BaseAPIController
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