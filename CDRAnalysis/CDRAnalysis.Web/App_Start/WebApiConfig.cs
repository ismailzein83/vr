using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CDRAnalysis.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Vanrise.Web.WebApiConfig.Register(config);
        }
    }
}
