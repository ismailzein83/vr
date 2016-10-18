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
            if (context.IncludeBlockedSupplierZones)
            {
                context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Max(itm => itm.SupplierRate);
            }
            else
            {
                var notBlockedSupplierZones = context.SupplierOptionDetail.SupplierZones.FindAll(itm => !itm.IsBlocked);
                if (notBlockedSupplierZones != null)
                    context.EffectiveRate = notBlockedSupplierZones.Max(itm => itm.SupplierRate);
            }
        }
    }
}
