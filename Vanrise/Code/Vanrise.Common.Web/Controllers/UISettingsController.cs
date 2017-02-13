using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "UISettings")]
    public class VRCommon_UISettingsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetUIParameters")]
        public UISettings GetUIParameters()
        {
            GeneralSettingsManager manager = new GeneralSettingsManager();
            return manager.GetUIParameters();
        }
    }
}