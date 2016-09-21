using System.Web;
using System.Web.Optimization;

namespace Xbooster.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);

            bundles.Add(new ScriptBundle("~/bundles/ModulesJavascripts").IncludeDirectory(
                "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                "~/Client/Modules/CDRComparison", "*.js", true).IncludeDirectory(
                "~/Client/Modules/ExcelConversion", "*.js", true).IncludeDirectory(
                "~/Client/Modules/XBooster_PriceListConversion", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_GenericData", "*.js", true));
        }
    }
}
