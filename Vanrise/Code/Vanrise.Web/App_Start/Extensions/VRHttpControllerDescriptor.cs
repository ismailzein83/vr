using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Vanrise.Common;

namespace Vanrise.Web
{
    public class VRHttpControllerDescriptor : HttpControllerDescriptor
    {
        public VRHttpControllerDescriptor(HttpConfiguration configuration, string controllerName, Type controllerType)
            : base(configuration, controllerName, controllerType)
        {
        }

        Dictionary<string, HttpActionDescriptor> _actionDescriptors;

        public HttpActionDescriptor GetActionDescriptor(string actionName, IHttpActionSelector actionSelector)
        {
            if (_actionDescriptors == null)
            {
                lock (this)
                {
                    var actionMappings = actionSelector.GetActionMapping(this);
                    actionMappings.ThrowIfNull("actionMappings");
                    var actionDescriptors = new System.Collections.Generic.Dictionary<string, System.Web.Http.Controllers.HttpActionDescriptor>();
                    foreach (var actionMapping in actionMappings)
                    {
                        actionDescriptors.Add(actionMapping.Key, actionMapping.First());
                    }
                    _actionDescriptors = actionDescriptors;
                }
            }
            HttpActionDescriptor actionDescriptor;
            if (!_actionDescriptors.TryGetValue(actionName, out actionDescriptor))
                throw new Exception(String.Format("action '{0}' not found in controller '{1}'", actionName, this.ControllerName));
            return actionDescriptor;
        }
    }

      //if (System.Configuration.ConfigurationManager.AppSettings["VRDontLoadDynamicAPIs"] != "true")
      //      {
      //          GlobalConfiguration.Configuration.Services.Replace(
      //          typeof(System.Web.Http.Dispatcher.IHttpControllerSelector),
      //          new VRHttpControllerSelector(config));

      //          GlobalConfiguration.Configuration.Services.Replace(
      //         typeof(IHttpActionSelector),
      //         new VRHttpActionSelector());
      //      }

}