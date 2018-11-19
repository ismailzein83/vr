//using System.Web;
//using System.Web.Optimization;
//using Vanrise.Common;
//using Vanrise.Common.Business;

//namespace Vanrise.Web
//{
//    public class BundleConfig
//    {
//        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
//        public static void RegisterBundles(BundleCollection bundles)
//        {
//            //BundleTable.EnableOptimizations = false;
//            //bundles.DirectoryFilter.Clear();
//            //bundles.IgnoreList.Clear();


//            #region Bower
//            #region JQuery
//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/JQuery").Include(
//                "~/Client/Libraries/Bower/jquery/dist/jquery.min.js"));
//            #endregion

//            #region Angular

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/Angular").Include(
//               "~/Client/Libraries/Bower/angular/angular.js",
//               "~/Client/Libraries/angular/angular-websocket.js",
//               "~/Client/Libraries/Bower/angular-cookies/angular-cookies.js",
//               "~/Client/Libraries/Bower/angular-route/angular-route.js",
//               "~/Client/Libraries/Bower/angular-messages/angular-messages.js",
//               "~/Client/Libraries/Bower/angular-animate/angular-animate.js",
//               "~/Client/Libraries/Bower/angular-sanitize/angular-sanitize.js",
//               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.js",
//               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.tpl.js",
//               "~/Client/Libraries/Bower/angular-notify/dist/angular-notify.js",
//               "~/Client/Libraries/Bower/angular-ivh-treeview/dist/ivh-treeview.js",
//               "~/Client/Libraries/Bower/Sortable/Sortable.js",
//               "~/Client/Libraries/Bower/Sortable/ng-sortable.js"
//              ));

//            #endregion

//            #region codemirror Script

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/codemirror").Include(
//               "~/Client/Libraries/Bower/codemirror/lib/codemirror.js",
//               "~/Client/Libraries/Bower/codemirror/mode/clike/clike.js"
//               ));

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/CodemirrorDirective").Include(
//               "~/Client/Libraries/Bower/angular-ui-codemirror/ui-codemirror.js"
//               ));

//            #endregion

//            #region codemirror Style

//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/CodeMirrorStyle").Include(
//               "~/Client/Libraries/Bower/codemirror/lib/codemirror.css"
//               ));

//            #endregion

//            #region Waves

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/waves").Include(
//               "~/Client/Libraries/Bower/waves/dist/waves.min.js"));

//            #endregion

//            #endregion

//            #region Bootstrap

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/bootstrap").IncludeDirectory(
//                "~/Client/Libraries/Bootstrap", "*.js", true));

//            #endregion


//            //AngularExtensions
//            //VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/AngularExtensions").IncludeDirectory(
//            //  "~/Client/Libraries/AngularExtensions", "*.js", true));

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/AngularExtensions").Include(
//                "~/Client/Libraries/AngularExtensions/jQueryFileUpload/js/jquery.ui.widget.js",
//                "~/Client/Libraries/AngularExtensions/BsDateTimePicker/moment-with-locales.js",
//                "~/Client/Libraries/AngularExtensions/BsDateTimePicker/bootstrap-datetimepicker.js").IncludeDirectory(
//                "~/Client/Libraries/AngularExtensions", "*.js", true));

//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/AngularExtensions").IncludeDirectory(
//               "~/Client/Libraries/AngularExtensions", "*.css", true));

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/HandsonTable").IncludeDirectory(
//             "~/Client/Libraries/HandsonTable", "*.js", true));

//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/HandsonTable").IncludeDirectory(
//                "~/Client/Libraries/HandsonTable", "*.css", true));

//            //Helpers
//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/helpers").IncludeDirectory(
//               "~/Client/Libraries/Helpers", "*.js", true));
//            //FontStyle
//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/FontStyle").IncludeDirectory(
//               "~/Client/FontStyle", "*.css", true));
//            //Style
//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/Styles").IncludeDirectory(
//               "~/Client/Styles", "*.css", true));

//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/Themes").Include(
//          "~/Client/Themes/theme.css").IncludeDirectory(
//           "~/Client/Themes", "*.css", true));


//            //Style bootstrap
//            VRWebContext.AddWebBundle(new VRStyleWebBundle("~/Content/bootstrap").IncludeDirectory(
//               "~/Client/Libraries/Bootstrap", "*.css", true).IncludeDirectory(
//               "~/Client/Libraries/Bootstrap", "*.png", true));



//            //HichChart
//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/highchart").Include(
//             "~/Client/Libraries/Charts/HighChartV6/highcharts.js",
//             "~/Client/Libraries/Charts/HighChartV6/highcharts-more.js",
//             "~/Client/Libraries/Charts/HighChartV6/highcharts-3d.js",
//             "~/Client/Libraries/Charts/HighChartV6/modules/exporting.js").IncludeDirectory(
//               "~/Client/Libraries/Charts/HighChartV6/modules", "*.js", true).IncludeDirectory(
//               "~/Client/Libraries/Charts/HighChartV6/Plugins", "*.js", true));

//            //Site
//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/Javascripts").IncludeDirectory(
//                "~/Client/Javascripts/Modules", "*.js", true).IncludeDirectory(
//                "~/Client/Javascripts/Constants", "*.js", true).IncludeDirectory(
//                "~/Client/Javascripts/Services", "*.js", true).IncludeDirectory(
//                "~/Client/Javascripts/Filters", "*.js", true).IncludeDirectory(
//                "~/Client/Javascripts/Controllers", "*.js", true).IncludeDirectory(
//                "~/Client/Javascripts/Directives", "*.js", true));

//            VRWebContext.AddWebBundle(new VRScriptWebBundle("~/bundles/BaseModulesJavascripts").IncludeDirectory(
//             "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
//             "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
//             "~/Client/Modules/VR_GenericData", "*.js", true));

//            AddModulesVRScriptWebBundle();
//        }

//        static void AddModulesVRScriptWebBundle()
//        {
//            var modulesJSBundle = new VRScriptWebBundle("~/bundles/ModulesJavascripts");
//            var extendedModulesNames = System.Configuration.ConfigurationManager.AppSettings["Web_ExtendedModulesNames"];
//            if (!string.IsNullOrWhiteSpace(extendedModulesNames))
//            {
//                string[] extendedModulesNamesArray = extendedModulesNames.Split(',');
//                foreach (var moduleName in extendedModulesNamesArray)
//                {
//                    modulesJSBundle.IncludeDirectory(string.Format("~/Client/Modules/{0}", moduleName), "*.js", true);
//                }
//            }

//            modulesJSBundle.IncludeDirectory("~/Client/Modules", "*.js", true);
//            VRWebContext.AddWebBundle(modulesJSBundle);
//        }
//    }
//    public class FileHashVersionBundleTransform : IBundleTransform
//    {
//        public void Process(BundleContext context, BundleResponse response)
//        {
//            foreach (var file in response.Files)
//            {

//                //encode file hash as a query string param
//                //string version = HttpServerUtility.UrlTokenEncode(fileHash);
//                var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
//                long version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
//                file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", version);
//            }
//        }
//    }
//}
