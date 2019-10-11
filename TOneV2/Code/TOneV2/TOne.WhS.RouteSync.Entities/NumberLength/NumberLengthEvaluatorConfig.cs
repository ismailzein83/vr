using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities.NumberLength
{
    public class NumberLengthEvaluatorConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_NumberLengthEvaluator";

        public string Editor { get; set; }
    }
}