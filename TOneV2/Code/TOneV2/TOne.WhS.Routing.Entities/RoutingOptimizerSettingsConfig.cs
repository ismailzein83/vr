using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingOptimizerSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RoutingOptimizerSettings";
        public string Editor { get; set; }

    }
}
