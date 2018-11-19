using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Vanrise.Common;

namespace Vanrise.Web
{
    public abstract class WebApiApplication : System.Web.HttpApplication
    {
        protected virtual void Application_Start()
        {
            Vanrise.Common.VRWebContext.SetWebContextReader(new DOTNETVRWebRequestContextReader()); 
            Vanrise.Common.Utilities.CompilePredefinedPropValueReaders();
            AreaRegistration.RegisterAllAreas();
            RegisterWebAPI();
            RegisterGlobalFilters();
            RegisterRoutes();           
            RegisterBundles();
        }

        protected virtual void RegisterWebAPI()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public virtual void RegisterGlobalFilters()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        public virtual void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        
        public virtual void RegisterBundles()
        {
            VRWebContext.RegisterApplicationWebBundles();
        }

        private class DOTNETVRWebRequestContextReader : IVRWebContextReader
        {            
            public string MapVirtualToPhysicalPath(string virtualPath)
            {
                HttpContext.Current.ThrowIfNull("HttpContext.Current");
                return HttpContext.Current.Server.MapPath(virtualPath);
            }

            public string GetCurrentRequestHeader(string headerKey)
            {
                return GetCurrentRequestWithValidate().Headers[headerKey];
            }

            public string GetCurrentRequestQueryString(string parameterName)
            {
                return GetCurrentRequestWithValidate().Params[parameterName];
            }

            public VRWebCookieCollection GetCurrentRequestCookies()
            {
                var currentRequest = GetCurrentRequestWithValidate();
                var cookies = new VRWebCookieCollection();
                if (currentRequest.Cookies != null)
                {
                    foreach (var cookieName in currentRequest.Cookies.AllKeys)
                    {
                        if (!cookies.ContainsKey(cookieName))
                            cookies.Add(cookieName, new VRWebCookie { Name = cookieName, Value = currentRequest.Cookies[cookieName].Value });
                    }
                }
                return cookies;
            }

            public string GetCurrentRequestBaseURL()
            {
                var requestURI = GetCurrentRequestWithValidate().Url;
                return requestURI.GetLeftPart(UriPartial.Authority);
            }

            public VRURLParts GetCurrentRequestURLParts()
            {
                var url = GetCurrentRequestWithValidate().Url;
                return new VRURLParts
                {
                    Scheme = url.Scheme,
                    Host = url.Host,
                    Port = url.Port
                };
            }

            HttpRequest GetCurrentRequestWithValidate()
            {
                HttpContext.Current.ThrowIfNull("HttpContext.Current");
                HttpContext.Current.Request.ThrowIfNull("HttpContext.Current.Request");
                return HttpContext.Current.Request;
            }
        }
    }
}
