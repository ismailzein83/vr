using System;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOption : IRouteOptionOrderTarget, IRouteOptionFilterTarget, IRouteOptionPercentageTarget
    {
        static RPRouteOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOption),
                "SupplierId", "SupplierRate", "Percentage", "OptionWeight", "SaleZoneId", "SupplierZoneMatchHasClosedRate");
        }

        public int SupplierId { get; set; }

        public long SaleZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public int? Percentage { get; set; }

        public decimal OptionWeight { get; set; }

        public int SupplierServiceWeight { get; set; }

        public bool SupplierZoneMatchHasClosedRate { get; set; }

        public SupplierStatus SupplierStatus { get; set; }

        public bool IsForced { get; set; }

        long? IRouteOptionOrderTarget.SaleZoneId { get { return this.SaleZoneId; } }

        long? IRouteOptionOrderTarget.SupplierZoneId { get { return null; } }
    }
}