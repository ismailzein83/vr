﻿using System;
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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRObjectTracking")]
    public class VRObjectTrackingController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredObjectTracking")]
        public object GetFilteredObjectTracking(Vanrise.Entities.DataRetrievalInput<VRLoggableEntityQuery> input)
        {
            VRObjectTrackingManager manager = new VRObjectTrackingManager();
            return GetWebResponse(input, manager.GetFilteredObjectTracking(input));
        }

        [HttpGet]
        [Route("GetViewHistoryItemClientActionName")]
        public string GetViewHistoryItemClientActionName(string uniqueName)
        {
            VRObjectTrackingManager manager = new VRObjectTrackingManager();
            return manager.GetViewHistoryItemClientActionName(uniqueName);
        }
    }
}