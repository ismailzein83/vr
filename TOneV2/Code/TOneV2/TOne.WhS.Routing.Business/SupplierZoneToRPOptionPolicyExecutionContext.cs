using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionPolicyExecutionContext : ISupplierZoneToRPOptionPolicyExecutionContext
    {
        public RPRouteOptionSupplier SupplierOptionDetail { get; internal set; }

        public decimal EffectiveRate { internal get; set; }
    }
}
