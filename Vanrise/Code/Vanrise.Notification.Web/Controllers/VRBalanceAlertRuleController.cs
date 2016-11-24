using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRBalanceAlertRule")]
    [JSONWithTypeAttribute]
    public class VRBalanceAlertRuleController : BaseAPIController
    {
        VRBalanceAlertRuleManager _manager = new VRBalanceAlertRuleManager();
        [HttpGet]
        [Route("GetVRBalanceAlertThresholdConfigs")]
        public IEnumerable<VRBalanceAlertRuleThresholdConfig> GetVRBalanceAlertThresholdConfigs(string extensionType)
        {
            return _manager.GetVRBalanceAlertThresholdConfigs(extensionType);
        }
    }
}