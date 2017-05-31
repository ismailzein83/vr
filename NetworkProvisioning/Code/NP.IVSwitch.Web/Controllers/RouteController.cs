﻿using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Route")]
    [JSONWithTypeAttribute]
    public class RouteController : BaseAPIController
    {
        RouteManager _manager = new RouteManager();

        [HttpPost]
        [Route("GetFilteredRoutes")]
        public object GetFilteredRoutes(Vanrise.Entities.DataRetrievalInput<RouteQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRoutes(input));
        }


        [HttpGet]
        [Route("GetRoute")]
        public Route GetRoute(int routeId)
        {
            return _manager.GetRoute(routeId);
        }
        [HttpGet]
        [Route("GetRoutesInfo")]
        public IEnumerable<RouteEntityInfo> GetRoutesInfo(string filter = null)
        {
            RouteInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<RouteInfoFilter>(filter) : null;
            return _manager.GetRoutesInfo(deserializedFilter);
        }
        [HttpPost]
        [Route("AddRoute")]
        public Vanrise.Entities.InsertOperationOutput<RouteDetail> AddRoute(RouteToAdd routeItem)
        {
            return _manager.AddRoute(routeItem);
        }

        [HttpPost]
        [Route("UpdateRoute")]
        public Vanrise.Entities.UpdateOperationOutput<RouteDetail> UpdateRoute(RouteToAdd routeItem)
        {
            return _manager.UpdateRoute(routeItem);
        }

        [HttpGet]
        [Route("GetSwitchDateTime")]
        public DateTime GetSwitchDateTime()
        {
            return _manager.GetSwitchDateTime();
        }
    }
}