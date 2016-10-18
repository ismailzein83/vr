using System;
using System.Linq;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionLowestRatePolicy : SupplierZoneToRPOptionPolicy
    {
        public override Guid ConfigId { get { return new Guid("e85f9e2f-1ce6-4cc3-9df9-b664e63826f5"); } }

        public override void Execute(ISupplierZoneToRPOptionPolicyExecutionContext context)
        {
            if (context.IncludeBlockedSupplierZones)
            {
                context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Min(itm => itm.SupplierRate);
            }
            else
            {
                var notBlockedSupplierZones = context.SupplierOptionDetail.SupplierZones.FindAll(itm => !itm.IsBlocked);
                if (notBlockedSupplierZones != null)
                    context.EffectiveRate = notBlockedSupplierZones.Min(itm => itm.SupplierRate);
            }
        }
    }
}
