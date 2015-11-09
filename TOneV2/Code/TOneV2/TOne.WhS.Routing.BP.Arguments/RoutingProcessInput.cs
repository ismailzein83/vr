using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime? EffectiveTime { get; set; }

        public RoutingDatabaseType RoutingDatabaseType { get; set; }

        public bool DivideProcessIntoSubProcesses { get; set; }

        public override string GetTitle()
        {
            return String.Format("Routing Process for Effective Time {0}", EffectiveTime);
        }
    }
}
