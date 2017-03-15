using System.Web;
using System.Web.Optimization;

namespace QualityMeasurement.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);
            var modulesJSBundle = Vanrise.Web.BundleConfig.CreateModulesScriptBundle().IncludeDirectory(
                "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                "~/Client/Modules/QM_CLITester", "*.js", true).IncludeDirectory(
                "~/Client/Modules/QM_BusinessEntity", "*.js", true);

            bundles.Add(modulesJSBundle);
        }
    }
}
