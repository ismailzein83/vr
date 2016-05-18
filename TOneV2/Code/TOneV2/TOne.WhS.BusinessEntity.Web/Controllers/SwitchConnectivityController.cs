﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchConnectivity")]
    public class SwitchConnectivityController : Vanrise.Web.Base.BaseAPIController
    {
        SwitchConnectivityManager _manager = new SwitchConnectivityManager();

        [HttpPost]
        [Route("GetFilteredSwitchConnectivities")]
        public object GetFilteredSwitchConnectivities(Vanrise.Entities.DataRetrievalInput<SwitchConnectivityQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwitchConnectivities(input));
        }

        [HttpGet]
        [Route("GetSwitchConnectivity")]
        public SwitchConnectivity GetSwitchConnectivity(int switchConnectivityId)
        {
            return _manager.GetSwitchConnectivity(switchConnectivityId);
        }

        [HttpPost]
        [Route("AddSwitchConnectivity")]
        public Vanrise.Entities.InsertOperationOutput<SwitchConnectivityDetail> AddSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            return _manager.AddSwitchConnectivity(switchConnectivity);
        }

        [HttpPost]
        [Route("UpdateSwitchConnectivity")]
        public Vanrise.Entities.UpdateOperationOutput<SwitchConnectivityDetail> UpdateSwitchConnectivity(SwitchConnectivity switchConnectivity)
        {
            return _manager.UpdateSwitchConnectivity(switchConnectivity);
        }
    }
}