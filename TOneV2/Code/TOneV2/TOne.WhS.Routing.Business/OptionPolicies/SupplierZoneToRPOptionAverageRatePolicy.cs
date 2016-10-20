using System;
using System.Linq;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SupplierZoneToRPOptionAverageRatePolicy : SupplierZoneToRPOptionPolicy
    {
        public override Guid ConfigId { get { return new Guid("6d584c11-ce52-4385-a871-3b59505d0f57"); } }

        public override void Execute(ISupplierZoneToRPOptionPolicyExecutionContext context)
        {
            if (context.IncludeBlockedSupplierZones)
            {
                context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Average(itm => itm.SupplierRate);
            }
            else
            {
                var notBlockedSupplierZones = context.SupplierOptionDetail.SupplierZones.FindAll(itm => !itm.IsBlocked);
                if (notBlockedSupplierZones != null && notBlockedSupplierZones.Count() > 0)
                    context.EffectiveRate = notBlockedSupplierZones.Average(itm => itm.SupplierRate);
            }
        }
    }
}