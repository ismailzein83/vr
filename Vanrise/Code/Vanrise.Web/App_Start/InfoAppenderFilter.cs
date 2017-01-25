using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.Common.Business;
using System.Globalization;

namespace Vanrise.Web.App_Start
{
    public class InfoAppenderFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
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