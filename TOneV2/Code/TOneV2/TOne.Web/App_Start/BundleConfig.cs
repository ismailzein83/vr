﻿using System.Web;
using System.Web.Optimization;

namespace TOne.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            Vanrise.Web.BundleConfig.RegisterBundles(bundles);

            bundles.Add(new ScriptBundle("~/bundles/ModulesJavascripts").IncludeDirectory(
                "~/Client/Modules/Main", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Analytics", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BI", "*.js", true).IncludeDirectory(
                "~/Client/Modules/Routing", "*.js", true).IncludeDirectory(
                "~/Client/Modules/BusinessEntity", "*.js", true));
        }
    }
}
