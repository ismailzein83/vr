using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionSettingsGroupConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RouteOptionSettingsGroup";
        public string Editor { get; set; }
    }
}
