﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteRuleSettings")]
    [JSONWithTypeAttribute]
    public class RouteRuleSettingsController
    {
        [HttpGet]
        [Route("GetRouteOptionSettingsGroupTemplates")]
        public List<TemplateConfig> GetRouteOptionSettingsGroupTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionSettingsGroupTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionOrderSettingsTemplates")]
        public List<TemplateConfig> GetRouteOptionOrderSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionOrderSettingsTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionFilterSettingsTemplates")]
        public List<TemplateConfig> GetRouteOptionFilterSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionFilterSettingsTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionPercentageSettingsTemplates")]
        public List<TemplateConfig> GetRouteOptionPercentageSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionPercentageSettingsTemplates();
        }
    }
}