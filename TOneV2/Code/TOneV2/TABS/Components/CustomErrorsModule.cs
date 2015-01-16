using System;
using System.Web;
using log4net;

namespace TABS.Components
{
    /// <summary>
    /// Summary description for CustomErrorModule
    /// </summary>
    public class CustomErrorsModule : IHttpModule
    {
        public static readonly string Url_Key = "T.One-LastError-URL";
        public static readonly string ErrorMessage_Key = "T.One-LastError-Message";
        public static readonly string FullError_Key = "T.One-LastError-FullError";

        protected static ILog log = log4net.LogManager.GetLogger("WebSite.Global");

        public CustomErrorsModule()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region IHttpModule Members

        public void Init(HttpApplication app)
        {
            app.Error += new EventHandler(app_Error);
        }

        void app_Error(object sender, EventArgs e)
        {
            // At this point we have information about the error
            HttpContext ctx = HttpContext.Current;
            if (ctx != null && ctx.Session != null)
            {
                HttpResponse response = ctx.Response;
                Exception exception = ctx.Server.GetLastError();
                log.Error(string.Format("Unhandled Error in {0}", ctx.Request.Url), exception);
                ctx.Session[Url_Key] = ctx.Request.Url.ToString();
                ctx.Session[ErrorMessage_Key] = exception.Message;
                ctx.Session[FullError_Key] = exception.ToString();
                ctx.Server.ClearError();
                ctx.Response.Redirect("~/ErrorPage.aspx", true);
            }
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
