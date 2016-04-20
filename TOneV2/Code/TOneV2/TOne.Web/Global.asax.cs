﻿    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TOne.Web
{
    public class WebApiApplication : Vanrise.Web.WebApiApplication
    {
        protected override void RegisterWebAPI()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public override void RegisterGlobalFilters()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        public override void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public override void RegisterBundles()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
