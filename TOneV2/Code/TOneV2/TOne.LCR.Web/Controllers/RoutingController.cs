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
    [CustomJSONTypeHandlingAttribure]
    public class RoutingController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public string GetTest(string prm)
        {
            return prm + " returned";
        }

        [HttpPost]
        public IEnumerable<RouteDetailModel> GetRoutes(GetFiltertedRoutesInput input)
        {
            RouteManager manager = new RouteManager();
            return Mappers.MapRouteDetails(manager.GetRoutes(input.Filter.CustomerIds, input.Filter.Code, input.Filter.ZoneIds, input.FromRow, input.ToRow, input.IsDescending, Enum.GetName(typeof(RouteDetailFilterOrder), input.OrderBy)));
        }
    }

    #region Argument Classes

    public class GetFiltertedRoutesInput
    {
        public RoutingFilter Filter { get; set; }
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
