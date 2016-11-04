using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum RoutingStatus { Enabled = 0, BlockedOutBound = 1 }
    public class CarrierAccountSupplierSettings
    {
        public RoutingStatus RoutingStatus { get; set; }

        public List<ZoneService> DefaultServices { get; set; }

        public int TimeZoneId { get; set; }

        public bool IncludeProcessingTimeZone { get; set; }
    }
}
