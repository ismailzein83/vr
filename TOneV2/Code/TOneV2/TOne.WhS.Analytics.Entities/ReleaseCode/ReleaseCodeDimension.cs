using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum ReleaseCodeDimension
    {
        [Description("MasterPlanZoneID")]
        MasterZone = 0,
        [Description("SupplierID")]
        Supplier = 1,
        [Description("PortIN")]
        PortIn = 4,
        [Description("PortOUT")]
        PortOut = 5
    }
}
