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
            if (supplierZones == null || !supplierZones.Any())
                return;

            var selectedSupplierZone = supplierZones.First();

            foreach (var supplierZone in supplierZones)
            {
                if (supplierZone.SupplierRate > selectedSupplierZone.SupplierRate)
                    selectedSupplierZone = supplierZone;
            }

            context.SupplierZoneId = selectedSupplierZone.SupplierZoneId;
            context.SupplierServicesIds = selectedSupplierZone.ExactSupplierServiceIds;
            context.EffectiveRate = selectedSupplierZone.SupplierRate;
        }
    }
}
