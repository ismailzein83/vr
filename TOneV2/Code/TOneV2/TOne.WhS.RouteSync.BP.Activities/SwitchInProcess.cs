using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class SwitchInProcess
    {
        public SwitchInfo Switch { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }

        public BaseQueue<RouteBatch> RouteQueue { get; set; }
    }
}
