using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionPolicyExecutionContext : ISupplierZoneToRPOptionPolicyExecutionContext
    {
        public RPRouteOptionSupplier SupplierOptionDetail { get; internal set; }

        public long? SupplierZoneId { get; set; }

        public HashSet<int> SupplierServicesIds { get; set; }

        public decimal EffectiveRate { internal get; set; }
    }
}
