using System;
using System.Linq;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionHighestRatePolicy : SupplierZoneToRPOptionPolicy
    {
        public override Guid ConfigId { get { return new Guid("cb8cc5ed-afda-4ed7-882d-1377666c141e"); } }

        public override void Execute(ISupplierZoneToRPOptionPolicyExecutionContext context)
        {
            context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Max(itm => itm.SupplierRate);
        }
    }
}
