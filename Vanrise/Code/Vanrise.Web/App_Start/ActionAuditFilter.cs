using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.Common.Business;

namespace Vanrise.Web.App_Start
{
    public class ActionAuditFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                string url = actionExecutedContext.Request.RequestUri.AbsolutePath.ToString();
                var uri = actionExecutedContext.Request.RequestUri;
                string host = uri.GetLeftPart(UriPartial.Authority);
                new UserActionAuditManager().AddUserActionAudit(url, host);
            }
            catch(Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }
        
    }
}