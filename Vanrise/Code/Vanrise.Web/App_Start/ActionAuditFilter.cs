using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Web.App_Start
{
    public class ActionAuditFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        static VRActionAuditManager s_manager = new VRActionAuditManager();
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                string url = actionExecutedContext.Request.RequestUri.AbsolutePath.ToString();
                var uri = actionExecutedContext.Request.RequestUri;
                string host = uri.GetLeftPart(UriPartial.Authority);
                new UserActionAuditManager().AddUserActionAudit(url, host);
                VRActionAuditAttribute methodAuditAttribute = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<VRActionAuditAttribute>().FirstOrDefault();
                if (methodAuditAttribute != null)
                {
                    var context = new VRActionAuditAttributeContext
                        {
                            ActionURL = url,
                            ActionArguments = actionExecutedContext.ActionContext.ActionArguments
                        };
                    methodAuditAttribute.GetAuditDetails(context);
                    context.ActionName.ThrowIfNull("context.ActionName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                    if (context.ModuleName == null || context.EntityName == null)
                    {
                        VRActionAuditAttribute controllerAuditAttribute = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<VRActionAuditAttribute>().FirstOrDefault();
                        controllerAuditAttribute.ThrowIfNull("controllerAuditAttribute");
                        controllerAuditAttribute.GetAuditDetails(context);
                        context.ModuleName.ThrowIfNull("context.ModuleName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                        context.EntityName.ThrowIfNull("context.EntityName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                    }
                    //LoggerFactory.GetLogger().WriteInformation("Action Audit: Module '{0}' Entity '{1}' Action '{2}' ObjectId '{3}'", context.ModuleName, context.EntityName, context.ActionName, context.ObjectId);
                    s_manager.AuditAction(host, context.ModuleName, context.EntityName, context.ActionName, context.ObjectId, context.ActionDescription);
                    //new UserActionAuditManager().AddUserActionAudit(url, host);
                }
            }
            catch (Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }

        private class VRActionAuditAttributeContext : IVRActionAuditAttributeContext
        {
            public string ActionDescription
            {
                set;
                get;
            }

            public string ActionName
            {
                set;
                get;
            }

            public string ActionURL
            {
                set;
                get;
            }

            public string EntityName
            {
                set;
                get;
            }

            public string ModuleName
            {
                set;
                get;
            }

            public string ObjectId
            {
                set;
                get;
            }

            public Dictionary<string, Object> ActionArguments { get; set; }

            public T GetActionArgument<T>(string argumentName)
            {
                this.ActionArguments.ThrowIfNull("ActionArguments");
                Object arg;
                if(!this.ActionArguments.TryGetValue(argumentName, out arg))
                {
                    throw new Exception(String.Format("Argument '{0}' is not found", argumentName));
                }
                if (arg != null)
                {
                    if (arg is T)
                        return (T)arg;
                    else
                        throw new Exception(String.Format("'{0}' is not of type '{1}'. it is of type '{2}'", argumentName, typeof(T), arg.GetType()));
                }
                else
                    return default(T);
            }
        }

    }
}