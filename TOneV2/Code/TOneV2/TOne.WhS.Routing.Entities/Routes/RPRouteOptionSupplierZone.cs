using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BaseRPRouteOptionSupplier
    {
        public long SupplierZoneId { get; set; }

        public long SupplierRateId { get; set; }

        public Decimal SupplierRate { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int? ExecutedRuleId { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsForced { get; set; }
    }
    public class RPRouteOptionSupplierZone : BaseRPRouteOptionSupplier
    {

    }

    public class RPRouteOptionSupplierCode : BaseRPRouteOptionSupplier
    {
        public string SupplierCode { get; set; }
    }
}