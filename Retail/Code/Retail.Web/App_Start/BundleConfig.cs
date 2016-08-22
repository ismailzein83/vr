using System.Web;
using System.Web.Optimization;

namespace Retail.Web
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
                "~/Client/Modules/Retail_BusinessEntity", "*.js", true).IncludeDirectory(
                 "~/Client/Modules/Integration", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Queueing", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Runtime", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_GenericData", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Retail_Voice", "*.js", true).IncludeDirectory(
                 "~/Client/Modules/Analytic", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Rules", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_BusinessEntity", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Retail_SMS", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Retail_Data", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_AccountBalance", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_Notification", "*.js", true).IncludeDirectory(
                "~/Client/Modules/VR_BEBridge", "*.js", true).IncludeDirectory(
                "~/Client/Modules/WhS_Routing", "*.js", true));
        }
    }
}