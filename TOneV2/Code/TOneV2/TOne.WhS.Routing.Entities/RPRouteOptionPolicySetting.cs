using System;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionPolicySetting : ExtensionConfiguration
    {
        public string Editor { get; set; }

        public string BehaviorFQTN { get; set; }

        public bool IsDefault { get; set; }

        public bool ShowSupplierZone { get; set; }
    }
}
