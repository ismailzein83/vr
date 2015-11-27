using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionAverageRatePolicy : SupplierZoneToRPOptionPolicy
    {
        public override void Execute(ISupplierZoneToRPOptionPolicyExecutionContext context)
        {
            context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Average(itm => itm.SupplierRate);
        }
    }
}
