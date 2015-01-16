using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Plugins.Framework
{
    public class PluginResourceHandler : System.Web.IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        Dictionary<string, byte[]> ResourceCache = new Dictionary<string, byte[]>();

        public void ProcessRequest(System.Web.HttpContext context)
        {
            var assemblyPlugin = PluginManager.LoadedPlugins.Where(lp => lp.Second.ID.Equals(context.Request["plugin"])).FirstOrDefault();
            if (assemblyPlugin != null)
            {
                string resourceName = context.Request["resource"];
                string desiredType = context.Request["type"];
                bool forceDownload = "True".Equals(context.Request["forceDownload"]);
                PluginResource resrouce = null;
                if (assemblyPlugin.Second.GetResources().TryGetValue(resourceName, out resrouce))
                {
                    context.Response.Clear();

                    if (string.IsNullOrEmpty(desiredType))
                    {
                        desiredType = "application/binary";
                    }
                    context.Response.AppendHeader("Content-Type", desiredType);
                    if (forceDownload) context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + resourceName + "\"");
                    context.Response.AppendHeader("Content-Length", resrouce.Contents.Length.ToString());
                    context.Response.BinaryWrite(resrouce.Contents);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            context.Response.Write("Invalid Plugin Resource Path");
            context.Response.End();
        }

        public static string GetPluginResourceUrl(string pluginID, string resourceName, string resourceType, bool forceDownload)
        {
            return string.Format("T.One.PluginResource.axd?plugin={0}&resource={1}&type={2}&forceDownload={3}", pluginID, resourceName, resourceType, forceDownload);
        }

        #endregion
    }
}
