using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RouteOptionRuleSettingsType";
        public string Editor { get; set; }
        public int? Priority { get; set; }
    }
}
