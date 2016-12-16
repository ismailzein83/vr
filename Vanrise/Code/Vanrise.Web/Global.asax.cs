using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Vanrise.Web
{
    public abstract class WebApiApplication : System.Web.HttpApplication
    {
        protected virtual void Application_Start()
        {
            Vanrise.Common.Utilities.CompilePredefinedPropValueReaders();
            AreaRegistration.RegisterAllAreas();
            RegisterWebAPI();
            RegisterGlobalFilters();
            RegisterRoutes();
            RegisterBundles();
        }

        protected abstract void RegisterWebAPI();

        public abstract void RegisterGlobalFilters();

        public abstract void RegisterRoutes();

        public abstract void RegisterBundles();
    }
}
