using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteSyncDefinition
    {
        public int RouteSyncDefinitionId { get; set; }

        public string Name { get; set; }

        public RouteSyncDefinitionSettings Settings { get; set; }
    }

    public class RouteSyncDefinitionSettings
    {
        public RouteReader RouteReader { get; set; } 

        public List<string> SwitchIds { get; set; }
    }
}
