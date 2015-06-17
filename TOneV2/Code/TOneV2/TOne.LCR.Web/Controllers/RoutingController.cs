using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.LCR.Entities;
using TOne.LCR.Business;
using TOne.LCR.Web.Models;
using TOne.LCR.Web.ModelMappers;
using System.Web.Http.Controllers;

namespace TOne.LCR.Web.Controllers
{
    [TOne.Entities.SecureController("/Routing/RouteRuleManagement")]
    [CustomJSONTypeHandlingAttribure]
    public class RoutingController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public string GetTest(string prm)
        {
            return prm + " returned";
        }

  
        //[HttpGet]
        //[TOne.Entities.SecureAction("View")]
        //public IEnumerable<RouteDetail> GetRoutes(int pageNumber, int pageSize, string customerId, string code, int ourZoneId)
        //{
        //    RouteManager manager = new RouteManager();
        //    return manager.GetRoutes(customerId, code, ourZoneId == 0 ? (int?)null : ourZoneId).Skip(pageNumber * pageSize).Take(pageSize);
        //}
        [HttpGet]
        [TOne.Entities.SecureAction("View")]
        public IEnumerable<RouteDetailModel> GetRoutes(GetFiltertedRoutesInput filter)
        {
            RouteManager manager = new RouteManager();
            return Mappers.MapRouteDetails(manager.GetRoutes(filter.CustomerIds, filter.Code, filter.ZoneIds, filter.FromRow, filter.ToRow, filter.IsDescending, Enum.GetName(typeof(RouteDetailFilterOrder), filter.OrderBy)));
        }
    }

    #region Argument Classes
    public class GetFilteredRouteRulesInput
    {
        public List<string> RuleTypes { get; set; }
        public List<int> ZoneIds { get; set; }
        public string Code { get; set; }
        public List<string> CustomerIds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }

    public class GetFiltertedRoutesInput
    {
        public List<int> ZoneIds { get; set; }
        public string Code { get; set; }
        public List<string> CustomerIds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public RouteDetailFilterOrder OrderBy { get; set; }
        public bool IsDescending { get; set; }
    }
    #endregion

    public class CustomJSONTypeHandlingAttribure : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Clear();
            var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
            controllerSettings.Formatters.Add(jsonFormatter);
        }
    }

}
