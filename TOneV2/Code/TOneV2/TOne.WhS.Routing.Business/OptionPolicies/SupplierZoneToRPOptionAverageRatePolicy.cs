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
            context.EffectiveRate = context.SupplierOptionDetail.SupplierZones.Average(itm => itm.SupplierRate);
        }
    }
}