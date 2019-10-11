using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities.CodeCharge
{
    public class CodeChargeEvaluatorConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_CodeChargeEvaluator";

        public string Editor { get; set; }
    }
}
