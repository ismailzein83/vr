using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using TOne.Web.Online.Security;

namespace TOne.Web.Online
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            AuthenticationHandler authenticationHandler = new AuthenticationHandler() { InnerHandler = new HttpControllerDispatcher(config) };
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
              name: "Authentication",
              routeTemplate: "api/security/{action}",
              defaults: new { controller = "security" }
          );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: authenticationHandler
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
