using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Vanrise.Common;

namespace Vanrise.Web
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            LoggerFactory.GetExceptionLogger().WriteException(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}