using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierCodeMatch
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public string SupplierCode { get; set; }
    }

    public class SupplierCodeMatchWithRate
    {
        public SupplierCodeMatch CodeMatch { get; set; }

        public Decimal RateValue { get; set; }

        public HashSet<int> SupplierServiceIds { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }
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
