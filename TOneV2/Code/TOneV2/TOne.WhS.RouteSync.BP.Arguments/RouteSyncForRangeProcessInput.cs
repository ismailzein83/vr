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
            throw new NotImplementedException();
        }

        public int RouteSyncDefinitionId { get; set; }

        public RouteRangeType RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }
    }
}
