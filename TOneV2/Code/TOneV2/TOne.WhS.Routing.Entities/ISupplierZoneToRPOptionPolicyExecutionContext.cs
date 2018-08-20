using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface ISupplierZoneToRPOptionPolicyExecutionContext
    {
        RPRouteOptionSupplier SupplierOptionDetail { get; }

        long? SupplierZoneId { get; set; }

        HashSet<int> SupplierServicesIds { get; set; }

        Decimal EffectiveRate { set; }
    }
}
