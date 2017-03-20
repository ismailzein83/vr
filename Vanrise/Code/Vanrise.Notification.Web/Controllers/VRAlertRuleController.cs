﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRAlertRule")]
    [JSONWithTypeAttribute]
    public class VRAlertRuleController : BaseAPIController
    {
        VRAlertRuleManager _manager = new VRAlertRuleManager();

        [HttpPost]
        [Route("GetFilteredVRAlertRules")]
        public object GetFilteredVRAlertRules(Vanrise.Entities.DataRetrievalInput<VRAlertRuleQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRAlertRules(input));
        }

        [HttpGet]
        [Route("GetVRAlertRule")]
        public VRAlertRule GetVRAlertRule(long vrAlertRuleId)
        {
            return _manager.GetVRAlertRule(vrAlertRuleId,true);
        }

        [HttpPost]
        [Route("AddVRAlertRule")]
        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleDetail> AddVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            return _manager.AddVRAlertRule(vrAlertRuleItem);
        }

        [HttpPost]
        [Route("UpdateVRAlertRule")]
        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleDetail> UpdateVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            return _manager.UpdateVRAlertRule(vrAlertRuleItem);
        }
    }
}