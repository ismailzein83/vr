using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vanrise.Security.Business;

namespace Vanrise.Web.App_Start
{
    public class AuthorizationFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            Vanrise.Entities.AuthorizationAttribute att = actionContext.ActionDescriptor.GetCustomAttributes<Vanrise.Entities.AuthorizationAttribute>().FirstOrDefault();

            if (att != null && !SecurityContext.Current.IsAllowed(att.Permissions))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }

            base.OnAuthorization(actionContext);
        }
    }
}