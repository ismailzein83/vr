using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.RouteSync.BP.Arguments
{
    public class RouteSyncForRangeProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Route Sync For Range Process";
        }

        public int RouteSyncDefinitionId { get; set; }

        public RouteRangeType RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }

        public Dictionary<string, SwitchRouteSyncInitializationData> SwitchesInitializationData { get; set; }

        public Guid? SwitchesInitializationDataId { get; set; }
    }
}
