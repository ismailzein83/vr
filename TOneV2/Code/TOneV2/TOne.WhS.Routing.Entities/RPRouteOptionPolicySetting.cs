using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionPolicySetting : ExtensionConfiguration
    {
        public string Editor { get; set; }

        public string BehaviorFQTN { get; set; }

        public bool IsDefault { get; set; }
    }
}
