using System.Web;
using System.Web.Optimization;
using Vanrise.Common.Business;

namespace Vanrise.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.DirectoryFilter.Clear();
            bundles.IgnoreList.Clear();


            #region Bower
            #region JQuery
            bundles.Add(new ScriptBundle("~/bundles/JQuery").Include(
                "~/Client/Libraries/Bower/jquery/dist/jquery.min.js"));
            #endregion

            #region Angular

            bundles.Add(new ScriptBundle("~/bundles/Angular").Include(
               "~/Client/Libraries/Bower/angular/angular.js",
               "~/Client/Libraries/angular/angular-websocket.js",
               "~/Client/Libraries/Bower/angular-cookies/angular-cookies.js",
               "~/Client/Libraries/Bower/angular-route/angular-route.js",
               "~/Client/Libraries/Bower/angular-messages/angular-messages.js",
               "~/Client/Libraries/Bower/angular-animate/angular-animate.js",
               "~/Client/Libraries/Bower/angular-sanitize/angular-sanitize.js",
               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.js",
               "~/Client/Libraries/Bower/angular-strap/dist/angular-strap.tpl.js",
               "~/Client/Libraries/Bower/angular-ui-switch/angular-ui-switch.js",
               "~/Client/Libraries/Bower/angular-notify/dist/angular-notify.js",
               "~/Client/Libraries/Bower/angular-ivh-treeview/dist/ivh-treeview.js",
               "~/Client/Libraries/Bower/Sortable/Sortable.js",
               "~/Client/Libraries/Bower/Sortable/ng-sortable.js"
              ));

            #endregion

            #region codemirror Script

            bundles.Add(new ScriptBundle("~/bundles/codemirror").Include(
               "~/Client/Libraries/Bower/codemirror/lib/codemirror.js",
               "~/Client/Libraries/Bower/codemirror/mode/clike/clike.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/CodemirrorDirective").Include(
               "~/Client/Libraries/Bower/angular-ui-codemirror/ui-codemirror.js"
               ));

            #endregion

            #region codemirror Style

            bundles.Add(new StyleBundle("~/Content/CodeMirrorStyle").Include(
               "~/Client/Libraries/Bower/codemirror/lib/codemirror.css"
               ));

            #endregion

            #region Waves

            bundles.Add(new ScriptBundle("~/bundles/waves").Include(
               "~/Client/Libraries/Bower/waves/dist/waves.min.js"));

            #endregion

            #endregion

            #region Bootstrap

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").IncludeDirectory(
                "~/Client/Libraries/Bootstrap", "*.js", true));

            #endregion


            //AngularExtensions
            //bundles.Add(new ScriptBundle("~/bundles/AngularExtensions").IncludeDirectory(
            //  "~/Client/Libraries/AngularExtensions", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/AngularExtensions").Include(
          "~/Client/Libraries/AngularExtensions/BsDateTimePicker/moment-with-locales.js",
          "~/Client/Libraries/AngularExtensions/BsDateTimePicker/bootstrap-datetimepicker.js").IncludeDirectory(
            "~/Client/Libraries/AngularExtensions", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/AngularExtensions").IncludeDirectory(
               "~/Client/Libraries/AngularExtensions", "*.css", true));

            bundles.Add(new ScriptBundle("~/bundles/HandsonTable").IncludeDirectory(
             "~/Client/Libraries/HandsonTable", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/HandsonTable").IncludeDirectory(
                "~/Client/Libraries/HandsonTable", "*.css", true));

            //Helpers
            bundles.Add(new ScriptBundle("~/bundles/helpers").IncludeDirectory(
               "~/Client/Libraries/Helpers", "*.js", true));
            //FontStyle
            bundles.Add(new StyleBundle("~/Content/FontStyle").IncludeDirectory(
               "~/Client/FontStyle", "*.css", true));
            //Style
            bundles.Add(new StyleBundle("~/Content/Styles").IncludeDirectory(
               "~/Client/Styles", "*.css", true));

            bundles.Add(new StyleBundle("~/Content/Themes").Include(
          "~/Client/Themes/theme.css").IncludeDirectory(
           "~/Client/Themes", "*.css", true));


            //Style bootstrap
            bundles.Add(new StyleBundle("~/Content/bootstrap").IncludeDirectory(
               "~/Client/Libraries/Bootstrap", "*.css", true).IncludeDirectory(
               "~/Client/Libraries/Bootstrap", "*.png", true));



            //HichChart
            bundles.Add(new ScriptBundle("~/bundles/highchart").Include(
               "~/Client/Libraries/Charts/HichChart/highcharts.js",
               "~/Client/Libraries/Charts/HichChart/highcharts-more.js",
               "~/Client/Libraries/Charts/HichChart/highcharts-3d.js").IncludeDirectory(
                "~/Client/Libraries/Charts/HichChart/adapters", "*.js", true).IncludeDirectory(
                "~/Client/Libraries/Charts/HichChart/modules", "*.js", true).IncludeDirectory(
                "~/Client/Libraries/Charts/HichChart/Plugins", "*.js", true));

            //Site
            bundles.Add(new ScriptBundle("~/bundles/Javascripts").IncludeDirectory(
                "~/Client/Javascripts/Modules", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Constants", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Services", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Filters", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Controllers", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Directives", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/BaseModulesJavascripts").IncludeDirectory(
             "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
             "~/Client/Modules/Security", "*.js", true));

            foreach (var b in bundles)
                b.Transforms.Add(new FileHashVersionBundleTransform());
          
        }

        public static ScriptBundle CreateModulesScriptBundle()
        {
            var modulesJSBundle = new ScriptBundle("~/bundles/ModulesJavascripts");
            var extendedModulesNames = System.Configuration.ConfigurationManager.AppSettings["Web_ExtendedModulesNames"];
            if (!string.IsNullOrWhiteSpace(extendedModulesNames))
            {
                string[] extendedModulesNamesArray = extendedModulesNames.Split(',');
                foreach (var moduleName in extendedModulesNamesArray)
                {
                    modulesJSBundle.IncludeDirectory(string.Format("~/Client/Modules/{0}", moduleName), "*.js", true);
                }
            }

            modulesJSBundle.Transforms.Add(new FileHashVersionBundleTransform());
            return modulesJSBundle;
        }


    }
    public class FileHashVersionBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            foreach (var file in response.Files)
            {

                //encode file hash as a query string param
                //string version = HttpServerUtility.UrlTokenEncode(fileHash);
                var cacheSettingData = new GeneralSettingsManager().GetCacheSettingData();
                long version = cacheSettingData != null ? cacheSettingData.ClientCacheNumber : 0;
                file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", version);
            }
        }
    }
}
