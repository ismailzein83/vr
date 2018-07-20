using System.Web;
using System.Web.Optimization;

namespace CarrierPortal.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);

            var modulesJSBundle = Vanrise.Web.BundleConfig.CreateModulesScriptBundle().IncludeDirectory(
                "~/Client/Modules", "*.js", true);
                //"~/Client/Modules/Common", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Runtime", "*.js", true);

            bundles.Add(modulesJSBundle);
        }
    }
}
