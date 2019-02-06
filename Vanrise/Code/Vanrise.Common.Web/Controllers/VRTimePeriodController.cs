﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRTimePeriod")]
    [JSONWithTypeAttribute]
    public class VRTimePeriodController:BaseAPIController
    {
        VRTimePeriodManager _manager = new VRTimePeriodManager();

        [HttpGet]
        [Route("GetVRTimePeriodConfigs")]
        public IEnumerable<VRTimePeriodConfig> GetVRTimePeriodConfigs()
        {
            return _manager.GetVRTimePeriodConfigs();
        }
        [HttpPost]
        [Route("GetTimePeriod")]
        public DateTimeRange GetTimePeriod(TimePeriodInput timePeriodInput)
        {
            return _manager.GetTimePeriod(timePeriodInput.TimePeriod, timePeriodInput.EffectiveDate);
        }


    }
    public class TimePeriodInput
    {
        public VRTimePeriod TimePeriod { get; set; }  
        public DateTime EffectiveDate { get; set; }
    }
}