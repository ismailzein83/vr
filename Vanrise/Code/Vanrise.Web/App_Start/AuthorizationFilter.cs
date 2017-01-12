using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common;
using Vanrise.Security.Business;

namespace Vanrise.Web.App_Start
{
    public class AuthorizationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                var systemAction = new SystemActionManager().GetSystemAction(GetSystemActionName(actionContext));

                if (systemAction != null && systemAction.RequiredPermissions != null && !SecurityContext.Current.HasPermissionToActions(systemAction.Name))
                {
                    actionContext.Response = Utils.CreateResponseMessage(System.Net.HttpStatusCode.Forbidden, "you are not authorized to perform this request");
                }
            }
            catch (Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }

            base.OnAuthorization(actionContext);
        }

        string GetSystemActionName(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Type controller = actionContext.ControllerContext.Controller.GetType();

            var routePrefixAttribute = controller.GetCustomAttribute(typeof(System.Web.Http.RoutePrefixAttribute)) as System.Web.Http.RoutePrefixAttribute;
            var actionMethod = controller.GetMethods().FindRecord(itm => itm.Name == actionContext.ActionDescriptor.ActionName);
            var routeAttribute = actionMethod.GetCustomAttribute(typeof(System.Web.Http.RouteAttribute)) as System.Web.Http.RouteAttribute;

            // This check was added in case the controller doesn't apply the RoutePrefix and/or Route attributes
            // If either attribute is null, an empty string is returned to GetSystemAction as it expects a key != null
            // This check should be removed after all system actions have been defined in the database, and a specific exception should be thrown if the system action wasn't found
            return (routePrefixAttribute != null && routeAttribute != null) ?
                String.Format("{0}/{1}", routePrefixAttribute.Prefix.Replace("api/", String.Empty), routeAttribute.Template) : String.Empty;
        }
    }
}