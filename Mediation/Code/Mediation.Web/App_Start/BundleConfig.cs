using System.Web;
using System.Web.Optimization;

namespace Mediation.Web
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
                "~/Client/Modules/Integration", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Queueing", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_GenericData", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Mediation_Generic", "*.js", true);
            bundles.Add(modulesJSBundle);
        }
    }
}
