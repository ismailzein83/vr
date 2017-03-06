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
                    var actionAttributecontext = new VRActionAuditAttributeContext
                        {
                            ActionURL = url,
                            ActionArguments = actionExecutedContext.ActionContext.ActionArguments
                        };
                    methodAuditAttribute.GetAuditDetails(actionAttributecontext);
                    string moduleName = actionAttributecontext.ModuleName;
                    string entityName = actionAttributecontext.EntityName;
                    string actionName = actionAttributecontext.ActionName;
                    string objectId = actionAttributecontext.ObjectId;
                    string objectName = actionAttributecontext.ObjectName;
                    string actionDescription = actionAttributecontext.ActionDescription;
                    Type objectNameResolverType = methodAuditAttribute.ObjectNameResolverType;
                    actionName.ThrowIfNull("actionName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                    if (moduleName == null || entityName == null)
                    {
                        VRActionAuditAttribute controllerAuditAttribute = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<VRActionAuditAttribute>().FirstOrDefault();
                        controllerAuditAttribute.ThrowIfNull("controllerAuditAttribute");
                        var controllerAttributecontext = new VRActionAuditAttributeContext
                        {
                            ActionURL = url,
                            ActionArguments = actionExecutedContext.ActionContext.ActionArguments
                        };
                        controllerAuditAttribute.GetAuditDetails(controllerAttributecontext);
                        if (moduleName == null) moduleName = controllerAttributecontext.ModuleName;
                        if (entityName == null) entityName = controllerAttributecontext.EntityName;
                        if (objectNameResolverType == null) objectNameResolverType = controllerAuditAttribute.ObjectNameResolverType;
                        moduleName.ThrowIfNull("moduleName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                        entityName.ThrowIfNull("entityName", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                    }
                    if (objectName == null && objectId != null)
                    {
                        if (objectNameResolverType != null)
                        {
                            Object objectNameResolverAsObj = Activator.CreateInstance(objectNameResolverType);
                            IVRActionObjectNameResolver objectNameResolver = objectNameResolverAsObj.CastWithValidate<IVRActionObjectNameResolver>("objectNameResolverAsObj", actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
                            var objectNameResolverContext = new VRActionObjectNameResolverContext { ObjectId = objectId };
                            objectName = objectNameResolver.GetObjectName(objectNameResolverContext);
                        }
                        else
                        {
                            objectName = objectId;
                        }
                    }
                    //LoggerFactory.GetLogger().WriteInformation("Action Audit: Module '{0}' Entity '{1}' Action '{2}' ObjectId '{3}'", context.ModuleName, context.EntityName, context.ActionName, context.ObjectId);
                    s_manager.AuditAction(host, moduleName, entityName, actionName, objectId, objectName, actionDescription);
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
            
            public string ObjectName
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

        public class VRActionObjectNameResolverContext : IVRActionObjectNameResolverContext
        {
            public string ObjectId
            {
                get;
                set;
            }
        }


    }
}