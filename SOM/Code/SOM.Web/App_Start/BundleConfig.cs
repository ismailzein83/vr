using System.Web;
using System.Web.Optimization;

namespace SOM.Web
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
                "~/Client/Modules/VR_GenericData", "*.js", true).IncludeDirectory(
                 "~/Client/Modules/Analytic", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Notification", "*.js", true);
            bundles.Add(modulesJSBundle);
        }
    }
}
