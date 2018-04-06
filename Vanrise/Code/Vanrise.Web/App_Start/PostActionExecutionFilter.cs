using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Vanrise.Common.Business;

namespace Vanrise.Web.App_Start
{
    public class PostActionExecutionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(actionExecutedContext.Exception);
                if (!new Security.Business.SecurityManager().GetExactExceptionMessage())
                    throw new Exception("Unexpected error occurred. Please consult technical support.");
            }

            try
            {
                if (actionExecutedContext.Response != null && actionExecutedContext.Response.Headers != null)
                    actionExecutedContext.Response.Headers.Add("ServerDate", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }
    }
}