using System.Web;
using System.Web.Optimization;

namespace Demo.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);

            bundles.Add(new ScriptBundle("~/bundles/ModulesJavascripts").IncludeDirectory(
                "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Integration", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Demo_Module", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Demo_BestPractices", "*.js", true));
        }
    }
}
