using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface ISupplierZoneToRPOptionPolicyExecutionContext
    {
        RPRouteOptionSupplier SupplierOptionDetail { get; }

        Decimal EffectiveRate { set; }
    }
}
