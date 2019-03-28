﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierCodeMatch
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierCodeSourceId { get; set; }
    }

    public class SupplierCodeMatchWithRate
    {
        public SupplierCodeMatch CodeMatch { get; set; }

        public int? DealId { get; set; }

        public Decimal RateValue { get; set; }

        public HashSet<int> SupplierServiceIds { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public int SupplierServiceWeight { get; set; }

        public long? SupplierRateId { get; set; }

        public DateTime? SupplierRateEED { get; set; }
    }

    public class SupplierCodeMatches
    {
        public int SupplierId { get; set; }

        public Decimal AvgRate { get; set; }

        public List<SupplierCodeZoneMatch> ZoneMatches { get; set; }
    }

    public class SupplierCodeZoneMatch
    {
        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal Rate { get; set; }
    }

    public class SupplierCodeMatchBySupplier : Dictionary<int, List<SupplierCodeMatch>>
    {
    }

    public class SupplierCodeMatchWithRateBySupplier : Dictionary<int, SupplierCodeMatchWithRate>
    {
    }

    public class SupplierCodeMatchesWithRateBySupplier : Dictionary<int, List<SupplierCodeMatchWithRate>>
    {
    }
}
