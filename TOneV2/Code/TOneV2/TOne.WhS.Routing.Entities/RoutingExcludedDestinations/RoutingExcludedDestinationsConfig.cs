using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingExcludedDestinationsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RoutingExcludedDestinations";

        public string Editor { get; set; }
    }
}