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
            var modulesJSBundle = new ScriptBundle("~/bundles/ModulesJavascripts").IncludeDirectory(
                "~/Client/Modules/Common", "*.js", true).IncludeDirectory(
                //   "~/Client/Modules/Main", "*.js", true).IncludeDirectory(
                //  "~/Client/Modules/Analytics", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BI", "*.js", true).IncludeDirectory(
                //  "~/Client/Modules/Routing", "*.js", true).IncludeDirectory(
                //   "~/Client/Modules/BusinessEntity", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Security", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Integration", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                //    "~/Client/Modules/Billing", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Queueing", "*.js", true).IncludeDirectory(
                //     "~/Client/Modules/CDR", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_CodePreparation", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_BusinessEntity", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_SupplierPriceList", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Sales", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Routing", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Analytics", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_GenericData", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Analytic", "*.js", true).IncludeDirectory(
                "~/Client/Modules/ExcelConversion", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Reprocess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_RouteSync", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Invoice", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Invoice", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Notification", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Deal", "*.js", true);
            var extendedModulesNames = System.Configuration.ConfigurationManager.AppSettings["Web_ExtendedModulesNames"];
            if (!string.IsNullOrWhiteSpace(extendedModulesNames))
            {
                string[] extendedModulesNamesArray = extendedModulesNames.Split(',');
                foreach(var moduleName in extendedModulesNamesArray)
                {
                    modulesJSBundle.IncludeDirectory(string.Format("~/Client/Modules/{0}", moduleName), "*.js", true);
                }
            }

            bundles.Add(modulesJSBundle);

            
        }
    }
}
