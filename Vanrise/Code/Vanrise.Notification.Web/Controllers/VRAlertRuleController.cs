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
        VRAlertRuleTypeManager _typeManager = new VRAlertRuleTypeManager();
        [HttpPost]
        [Route("GetFilteredVRAlertRules")]
        public object GetFilteredVRAlertRules(Vanrise.Entities.DataRetrievalInput<VRAlertRuleQuery> input)
        {
            if (!_typeManager.DoesUserHaveViewAccess())
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredVRAlertRules(input));
        }

        [HttpGet]
        [Route("GetVRAlertRule")]
        public VRAlertRule GetVRAlertRule(long vrAlertRuleId)
        {
            return _manager.GetVRAlertRule(vrAlertRuleId,true);
        }
        [HttpGet]
        [Route("DoesUserHaveAddAlertRulePermission")]
        public bool DoesUserHaveAddAlertRulePermission()
        {
            return _typeManager.DoesUserHaveAddAccess();
        }
        [HttpPost]
        [Route("AddVRAlertRule")]
        public object AddVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            if (!_typeManager.DoesUserHaveAddAccess(vrAlertRuleItem.RuleTypeId))
                return GetUnauthorizedResponse();
            return _manager.AddVRAlertRule(vrAlertRuleItem);
        }

        [HttpPost]
        [Route("UpdateVRAlertRule")]
        public object UpdateVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            if (!_typeManager.DoesUserHaveEditAccess(vrAlertRuleItem.RuleTypeId))
                return GetUnauthorizedResponse();
            return _manager.UpdateVRAlertRule(vrAlertRuleItem);
        }
    }
}