using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum RoutingStatus { Enabled=0, BlockedOutBound=1}
    public class CarrierAccountSupplierSettings
    {
        public RoutingStatus RoutingStatus { get; set; }
    }
}
