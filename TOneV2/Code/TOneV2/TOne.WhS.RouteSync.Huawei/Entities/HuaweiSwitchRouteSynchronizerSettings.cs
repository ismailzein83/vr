using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiSwitchRouteSynchronizerSettings : RouteSynchronizerSwitchSettings
    {
        public int NumberOfRetries { get; set; }
    }
}