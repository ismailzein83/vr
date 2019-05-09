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
            var supplierZones = context.SupplierOptionDetail.SupplierZones;
            if (supplierZones == null || supplierZones.Count == 0)
                return;

            RPRouteOptionSupplierZone selectedSupplierZone = null;

            foreach (var supplierZone in supplierZones)
            {
                if (supplierZone.IsBlocked && !context.IncludeBlockedZonesInCalculation)
                    continue;

                if (selectedSupplierZone != null && supplierZone.SupplierRate >= selectedSupplierZone.SupplierRate)
                    continue;

                selectedSupplierZone = supplierZone;
            }

            if (selectedSupplierZone == null)
                return;

            context.SupplierZoneId = selectedSupplierZone.SupplierZoneId;
            context.SupplierServicesIds = selectedSupplierZone.ExactSupplierServiceIds;
            context.EffectiveRate = selectedSupplierZone.SupplierRate;
        }
    }
}