using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleOptionFilterSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RouteRuleOptionFilterSettings";
        public string Editor { get; set; }
    }
}
