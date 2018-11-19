using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class VRWebContext
    {
        static IVRWebContextReader s_requestContextReader;

        static Dictionary<string, VRWebBundle> s_webBundles = new Dictionary<string, VRWebBundle>();

        public static void SetWebContextReader(IVRWebContextReader requestContextReader)
        {
            if (s_requestContextReader != null)
                throw new Exception("Request Context Reader already set");
            s_requestContextReader = requestContextReader;
        }

        public static bool IsInWebContext()
        {
            return s_requestContextReader != null;
        }

        public static string MapVirtualToPhysicalPath(string virtualPath)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.MapVirtualToPhysicalPath(virtualPath);
        }

        public static string GetCurrentRequestHeader(string headerKey)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestHeader(headerKey);
        }

        public static string GetCurrentRequestQueryString(string parameterName)
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestQueryString(parameterName);
        }

        public static VRWebCookie GetCurrentRequestCookie(string cookieName)
        {
            return GetCurrentRequestCookies().GetRecord(cookieName);
        }
        
        public static VRWebCookieCollection GetCurrentRequestCookies()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestCookies();
        }

        public static string GetCurrentRequestBaseURL()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestBaseURL();
        }

        public static VRURLParts GetCurrentRequestURLParts()
        {
            s_requestContextReader.ThrowIfNull("s_requestContextReader");
            return s_requestContextReader.GetCurrentRequestURLParts();
        }

        public static void AddWebBundle(VRWebBundle bundle)
        {
            s_webBundles.Add(bundle.Name, bundle);
        }

        public static List<string> GetWebBundlePaths(string bundleName)
        {
            VRWebBundle bundle;
            if (!s_webBundles.TryGetValue(bundleName, out bundle))
                throw new NullReferenceException($"bundle '{bundleName}'");
            return bundle.FilePaths;
        }

        public static void RegisterApplicationWebBundles()
        {
            #region Bower
            #region JQuery
            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/JQuery").Include(
                "~/Client/Libraries/Bower/jquery/dist/jquery.min.js"));
            #endregion

            #region Angular

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/Angular").Include(
               "~/Client/Libraries/Bower/angular/angular.js",
               "~/Client/Libraries/angular/angular-websocket.js",
               "~/Client/Libraries/Bower/angular-cookies/angular-cookies.js",
               "~/Client/Libraries/Bower/angular-route/angular-route.js",
               "~/Client/Libraries/Bower/angular-messages/angular-messages.js",
               "~/Client/Libraries/Bower/angular-animate/angular-animate.js",
               "~/Client/Libraries/Bower/angular-sanitize/angular-sanitize.js",
               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.js",
               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.tpl.js",
               "~/Client/Libraries/Bower/angular-notify/dist/angular-notify.js",
               "~/Client/Libraries/Bower/angular-ivh-treeview/dist/ivh-treeview.js",
               "~/Client/Libraries/Bower/Sortable/Sortable.js",
               "~/Client/Libraries/Bower/Sortable/ng-sortable.js"
              ));

            #endregion

            #region codemirror Script

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/codemirror").Include(
               "~/Client/Libraries/Bower/codemirror/lib/codemirror.js",
               "~/Client/Libraries/Bower/codemirror/mode/clike/clike.js"
               ));

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/CodemirrorDirective").Include(
               "~/Client/Libraries/Bower/angular-ui-codemirror/ui-codemirror.js"
               ));

            #endregion

            #region codemirror Style

            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/CodeMirrorStyle").Include(
               "~/Client/Libraries/Bower/codemirror/lib/codemirror.css"
               ));

            #endregion

            #region Waves

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/waves").Include(
               "~/Client/Libraries/Bower/waves/dist/waves.min.js"));

            #endregion

            #endregion

            #region Bootstrap

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/bootstrap").IncludeDirectory(
                "~/Client/Libraries/Bootstrap", "*.js", true));

            #endregion


            //AngularExtensions
            //VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/AngularExtensions").IncludeDirectory(
            //  "~/Client/Libraries/AngularExtensions", "*.js", true));

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/AngularExtensions").Include(
                "~/Client/Libraries/AngularExtensions/jQueryFileUpload/js/jquery.ui.widget.js",
                "~/Client/Libraries/AngularExtensions/BsDateTimePicker/moment-with-locales.js",
                "~/Client/Libraries/AngularExtensions/BsDateTimePicker/bootstrap-datetimepicker.js").IncludeDirectory(
                "~/Client/Libraries/AngularExtensions", "*.js", true));

            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/AngularExtensions").IncludeDirectory(
               "~/Client/Libraries/AngularExtensions", "*.css", true));

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/HandsonTable").IncludeDirectory(
             "~/Client/Libraries/HandsonTable", "*.js", true));

            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/HandsonTable").IncludeDirectory(
                "~/Client/Libraries/HandsonTable", "*.css", true));

            //Helpers
            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/helpers").IncludeDirectory(
               "~/Client/Libraries/Helpers", "*.js", true));
            //FontStyle
            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/FontStyle").IncludeDirectory(
               "~/Client/FontStyle", "*.css", true));
            //Style
            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/Styles").Include(
                "~/Client/Styles/jquery-ui.css")
                .IncludeDirectory("~/Client/Styles", "*.css", true));

            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/Themes").Include(
          "~/Client/Themes/theme.css").IncludeDirectory(
           "~/Client/Themes", "*.css", true));


            //Style bootstrap
            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/bootstrap").IncludeDirectory(
               "~/Client/Libraries/Bootstrap", "*.css", true).IncludeDirectory(
               "~/Client/Libraries/Bootstrap", "*.png", true));



            //HichChart
            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/highchart").Include(
             "~/Client/Libraries/Charts/HighChartV6/highcharts.js",
             "~/Client/Libraries/Charts/HighChartV6/highcharts-more.js",
             "~/Client/Libraries/Charts/HighChartV6/highcharts-3d.js",
             "~/Client/Libraries/Charts/HighChartV6/modules/exporting.js").IncludeDirectory(
               "~/Client/Libraries/Charts/HighChartV6/modules", "*.js", true).IncludeDirectory(
               "~/Client/Libraries/Charts/HighChartV6/Plugins", "*.js", true));

            //Site
            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/Javascripts").IncludeDirectory(
                "~/Client/Javascripts/Modules", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Constants", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Services", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Filters", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Controllers", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Directives", "*.js", true));

            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/BaseModulesJavascripts").IncludeDirectory(
             "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
             "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
             "~/Client/Modules/VR_GenericData", "*.js", true));

            AddModulesVRScriptWebBundle();
        }

        static void AddModulesVRScriptWebBundle()
        {
            var modulesJSBundle = new VRScriptWebBundle("~/bundles/ModulesJavascripts");
            var extendedModulesNames = System.Configuration.ConfigurationManager.AppSettings["Web_ExtendedModulesNames"];
            if (!string.IsNullOrWhiteSpace(extendedModulesNames))
            {
                string[] extendedModulesNamesArray = extendedModulesNames.Split(',');
                foreach (var moduleName in extendedModulesNamesArray)
                {
                    modulesJSBundle.IncludeDirectory(string.Format("~/Client/Modules/{0}", moduleName), "*.js", true);
                }
            }

            modulesJSBundle.IncludeDirectory("~/Client/Modules", "*.js", true);
            VRWebContext.AddWebBundle(modulesJSBundle);
        }

        #region Disabled Code for Future use with .Net Core

        //static string s_rootWebPhysicalPath;

        //static SortedList<int, VRWebVirtualDirectory> s_virtualDirectories = new SortedList<int, VRWebVirtualDirectory>(new VRVirtualDirectoryComparer());

        //private class VRVirtualDirectoryComparer : IComparer<int>
        //{
        //    public int Compare(int x, int y)
        //    {
        //        return -x.CompareTo(y);
        //    }
        //}

        //public static string MapVirtualToPhysicalPath(string virtualPath)
        //{
        //    string rootVirtualPath;
        //    return MapVirtualToPhysicalPath(virtualPath, out rootVirtualPath);
        //}

        //public static string MapVirtualToPhysicalPath(string virtualPath, out string rootVirtualPath)
        //{
        //    s_rootWebPhysicalPath.ThrowIfNull("s_rootWebPhysicalPath");
        //    virtualPath = virtualPath.TrimStart('~');
        //    string physicalPath = Path.Combine(s_rootWebPhysicalPath, virtualPath.Replace("/", @"\").TrimStart('\\'));
        //    if (Utilities.PhysicalPathExists(physicalPath))
        //    {
        //        rootVirtualPath = "";
        //        return physicalPath;
        //    }

        //    foreach (var virtualDirectory in s_virtualDirectories.Values)
        //    {
        //        if (virtualPath.StartsWith(virtualDirectory.VirtualPath))
        //        {
        //            physicalPath = Path.Combine(virtualDirectory.PhysicalPath, virtualPath.Replace(virtualDirectory.VirtualPath, "").Replace("/", @"\").TrimStart('\\'));
        //            if (Utilities.PhysicalPathExists(physicalPath))
        //            {
        //                rootVirtualPath = virtualDirectory.VirtualPath;
        //                return physicalPath;
        //            }
        //        }
        //    }
        //    throw new Exception($"Could not Find Physical Path of Virtual Path '{virtualPath}");
        //}

        //public static void SetRootWebPhysicalPath(string rootWebPhysicalPath)
        //{
        //    s_rootWebPhysicalPath = rootWebPhysicalPath;
        //}

        //public static void AddVirtualDirectory(string virtualPath, string physicalPath)
        //{
        //    s_virtualDirectories.Add(virtualPath.Length, new VRWebVirtualDirectory
        //    {
        //        VirtualPath = virtualPath,
        //        PhysicalPath = physicalPath
        //    });
        //}

        #endregion
    }

    //public class VRWebVirtualDirectory
    //{
    //    public string VirtualPath { get; set; }

    //    public string PhysicalPath { get; set; }
    //}

    public interface IVRWebContextReader
    {
        string MapVirtualToPhysicalPath(string virtualPath);

        string GetCurrentRequestHeader(string headerKey);

        string GetCurrentRequestQueryString(string parameterName);

        VRWebCookieCollection GetCurrentRequestCookies();

        string GetCurrentRequestBaseURL();

        VRURLParts GetCurrentRequestURLParts();
    }

    public class VRWebCookie
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class VRURLParts
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }

    public class VRWebCookieCollection : Dictionary<string, VRWebCookie>
    {
    }
}
