using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleCriteriaConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RouteRuleCriteriaType";
        public string Editor { get; set; }
        public bool IsDefault { get; set; }
    }
}
