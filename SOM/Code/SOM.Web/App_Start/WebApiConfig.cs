using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;

namespace SOM.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Vanrise.Web.WebApiConfig.Register(config);
        }
    }
}
