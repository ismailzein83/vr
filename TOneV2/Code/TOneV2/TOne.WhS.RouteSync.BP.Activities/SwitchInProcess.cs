using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class SwitchInProcess
    {
        public SwitchInfo Switch { get; set; }

        public Object InitializationData { get; set; }

        public RouteSyncDeliveryMethod SupportedDeliveryMethod { get; set; }
    }
}
