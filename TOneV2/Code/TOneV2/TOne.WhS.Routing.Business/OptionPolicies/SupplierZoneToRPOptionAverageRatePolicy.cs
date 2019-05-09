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
            var supplierZones = context.SupplierOptionDetail.SupplierZones;
            if (supplierZones == null || supplierZones.Count == 0)
                return;

            int numberOfZones = 0;
            decimal supplierRateSummation = 0;

            foreach (var supplierZone in supplierZones)
            {
                if (supplierZone.IsBlocked && !context.IncludeBlockedZonesInCalculation)
                    continue;

                numberOfZones++;
                supplierRateSummation += supplierZone.SupplierRate;
            }

            if (numberOfZones > 0)
                context.EffectiveRate = supplierRateSummation / numberOfZones;
        }
    }
}