using System.Web;
using System.Web.Optimization;

namespace TOne.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);
            var modulesJSBundle = Vanrise.Web.BundleConfig.CreateModulesScriptBundle().IncludeDirectory(
                "~/Client/Modules", "*.js", true);
                //"~/Client/Modules/Common", "*.js", true)
                //.IncludeDirectory(
                //"~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Integration", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Queueing", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_CodePreparation", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_BusinessEntity", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_SupplierPriceList", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_Sales", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_Routing", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_Analytics", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/VR_GenericData", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Analytic", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/ExcelConversion", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/Reprocess", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_RouteSync", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/VR_Invoice", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_Invoice", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/VR_Notification", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/WhS_Deal", "*.js", true).IncludeDirectory(
                //"~/Client/Modules/VR_BEBridge", "*.js", true);

            bundles.Add(modulesJSBundle);
        }
    }
}
