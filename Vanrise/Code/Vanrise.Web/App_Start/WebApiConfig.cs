using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Vanrise.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           // config.Filters.Add(new LicenseCheckFilter());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null//,
                // handler: authenticationHandler
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new MyDateConverter());
        }
    }

    //public class MyDateConverter : DateTimeConverterBase
    //{
    //    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
    //    {
    //        return DateTime.Parse(reader.Value as string);
    //    }

    //    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    //    {
    //        writer.WriteValue(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
    //    }
    //}


    //public class MyAuthorizationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    //{
    //    public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
    //    {
    //        //throw new Exception("License Key is expired. key is: ");
    //        base.OnAuthorization(actionContext);
    //    }
    //}
}
