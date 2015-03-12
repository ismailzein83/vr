using System.Web;
using System.Web.Optimization;

namespace TOne.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.DirectoryFilter.Clear();
            bundles.IgnoreList.Clear();
            //JQuery
            bundles.Add(new ScriptBundle("~/bundles/JQuery").IncludeDirectory(
                "~/Client/Libraries/JQuery", "*.js", true));

            //Angular
            //bundles.Add(new ScriptBundle("~/bundles/Angular").IncludeDirectory(
            //    "~/Client/Libraries/Angular", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Angular").Include(
               "~/Client/Libraries/Angular/angular.js",
               "~/Client/Libraries/Angular/angular-route.js",
               "~/Client/Libraries/Angular/angular-sanitize.js",              
                "~/Client/Libraries/Angular/angular-strap.js",
               "~/Client/Libraries/Angular/angular-strap.tpl.js",
              "~/Client/Libraries/Angular/sortable.js",
               "~/Client/Libraries/Angular/angular-ui-switch.js",
              "~/Client/Libraries/Angular/ng-sortable.js"));

            //Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").IncludeDirectory(
                "~/Client/Libraries/Bootstrap", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/bootstrap").IncludeDirectory(
                "~/Client/Libraries/Bootstrap", "*.css", true).IncludeDirectory(
                "~/Client/Libraries/Bootstrap", "*.png", true));

            //Semantic
            //bundles.Add(new ScriptBundle("~/bundles/Semantic").IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.js", true));

            //bundles.Add(new StyleBundle("~/Content/Semantic").IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.css", true));
            //.IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.png", true).IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.eot", true).IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.svg", true).IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.ttf", true).IncludeDirectory(
            //    "~/Client/Libraries/Semantic", "*.woff", true));

            //Site
            bundles.Add(new ScriptBundle("~/bundles/Javascripts").IncludeDirectory(
                "~/Client/Javascripts/Modules", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Controllers", "*.js", true).IncludeDirectory(
                "~/Client/Javascripts/Directives", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/Styles").IncludeDirectory(
               "~/Client/Styles", "*.css", true));
        }
    }
}
