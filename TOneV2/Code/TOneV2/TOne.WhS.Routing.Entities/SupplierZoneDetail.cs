using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierZoneDetail
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal EffectiveRateValue { get; set; }

        public HashSet<int> SupplierServiceIds { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int SupplierServiceWeight { get; set; }

        public long? SupplierRateId { get; set; }

        public DateTime? SupplierRateEED { get; set; }

        public int? DealId { get; set; }

        public int VersionNumber { get; set; }
    }

    public class SupplierZoneDetailByZone : Dictionary<long, SupplierZoneDetail>
    {

    }

    public class SupplierZoneDetailBatch
    {
        public List<SupplierZoneDetail> SupplierZoneDetails { get; set; }

    }
}
