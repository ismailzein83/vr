using System;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDataProviderConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_AnalyticDataProviderSettings";

        public string Editor { get; set; }
    }
}
