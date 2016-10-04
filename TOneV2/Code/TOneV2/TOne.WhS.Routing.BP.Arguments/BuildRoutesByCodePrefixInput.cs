using System;
using System.Collections;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildRoutesByCodePrefixInput : BaseProcessInputArgument
    {
        public int ParentWFRuntimeProcessId { get; set; }

        public Dictionary<string,int> SupplierCodeServiceRuntimeProcessIds { get; set; }

        public int RoutingDatabaseId { get; set; }

        public IEnumerable<CodePrefix> CodePrefixGroup { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public string CodePrefixGroupDescription { get; set; }

        public List<string> Switches { get; set; }

        public Dictionary<string, SwitchRouteSyncInitializationData> SwitchesInitializationData { get; set; }

        public bool BuildRouteSync { get; set; }

        public bool StoreCodeMatches { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle#: {0}", CodePrefixGroupDescription);
        }
    }
}