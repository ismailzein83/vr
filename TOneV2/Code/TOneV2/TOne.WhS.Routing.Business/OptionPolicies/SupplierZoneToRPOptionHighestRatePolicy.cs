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
            var supplierZones = context.SupplierOptionDetail.SupplierZones;
            if (supplierZones == null || supplierZones.Count == 0)
                return;

            RPRouteOptionSupplierZone selectedSupplierZone = null;

            foreach (var supplierZone in supplierZones)
            {
                if (supplierZone.IsBlocked && !context.IncludeBlockedZonesInCalculation)
                    continue;

                if (selectedSupplierZone != null && supplierZone.SupplierRate <= selectedSupplierZone.SupplierRate)
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
