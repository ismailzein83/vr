using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteSyncBPConfig: ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "TOne_WhS_RouteSync_RouteSyncBP";

        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }
}
