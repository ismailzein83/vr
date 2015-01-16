using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TABS.Plugins.Framework
{
    public class BasePage : Page
    {
        static bool? _IsAuthorizationEnabled;

        protected static bool IsAuthorizationEnabled
        {
            get
            {
                lock (typeof(BasePage))
                {
                    try
                    {
                        _IsAuthorizationEnabled = bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["IsAuthorizationEnabled"]);
                    }
                    catch
                    {
                        _IsAuthorizationEnabled = true;
                    }
                }
                return _IsAuthorizationEnabled.Value;
            }
        }
        protected override void OnPreInit(EventArgs e)
        {
            if (/*IsAuthorizationEnabled*/true)
            {
                if (SecurityEssentials.Web.Helper.CurrentWebUser.ID < 1)
                {
                    Response.Redirect("~/Login.aspx?to=" + Server.UrlEncode(Request.Url.AbsoluteUri), true);
                    return;
                }
                string[] resources =
                    SecurityEssentials.Web.Helper.CurrentWebUser["ACCESSIBLE_PLUGIN_RESOURCES"]
                    .Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                bool found = false;
                for (int i = 0; i < resources.Length && !found; i++)
                    found = (Request.Url.AbsolutePath.ToLower() == ResolveUrl(resources[i]).ToLower());
                if (!found)
                {
                    Response.Write("Unauthorized Access");
                    Response.End();
                }
            }
            base.OnPreInit(e);
        }
        /// <summary>
        /// Add an info message to the master page with a formatted html string
        /// </summary>
        /// <param name="htmlFormat">The html string (text) to format</param>
        /// <param name="parameters">The parameters to format the html string</param>
        public static void AddInfoMessage(System.Web.UI.Page page, string htmlFormat, params object[] parameters)
        {
            AddInfoMessage(page, string.Format(htmlFormat, parameters));
        }

        /// <summary>
        /// Add an info message to the master page
        /// </summary>
        /// <param name="html">The html string</param>
        public static void AddInfoMessage(System.Web.UI.Page page, string html)
        {
            try
            {
                if (page.Master != null)
                {
                    Literal control = page.Master.FindControl("lit_Message") as Literal;
                    control.Text += html;
                }
                else
                {
                    Literal control = page.FindControl("lit_Message") as Literal;
                    if (control != null) control.Text += html;
                }
            }
            catch
            {
            }
        }


        public static SecurityEssentials.User CurrentUser
        {
            get { return SecurityEssentials.Web.Helper.CurrentWebUser; }
        }

        /// <summary>
        /// Add an error message to the master page
        /// </summary>
        /// <param name="html">The html string</param>
        public static void AddErrorMessage(System.Web.UI.Page page, string html)
        {
            try
            {
                if (page.Master != null)
                {
                    Literal control = page.Master.FindControl("lit_Message") as Literal;
                    control.Text += string.Format("<span style='color: red'>{0}</span>", html);
                }
                else
                {
                    Literal control = page.FindControl("lit_Message") as Literal;
                    if (control != null) control.Text += string.Format("<span style='color: red'>{0}</span>", html);
                }
            }
            catch
            {
            }
        }
    }
}
