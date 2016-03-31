﻿using System.Web;
using System.Web.Optimization;

namespace CarrierPortal.Web
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
                "~/Client/Modules/CP_SupplierPricelist", "*.js", true).IncludeDirectory(
                "~/Client/Modules/CDRComparison", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessProcess", "*.js", true).IncludeDirectory(
                "~/Client/Modules/ExcelConversion", "*.js", true));
        }
    }
}
