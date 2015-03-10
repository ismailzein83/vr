using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCR.Entities
{
    public class CodeMatch
    {
        public string Code { get; set; }
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public long SupplierCodeId { get; set; }
        public int SupplierZoneId { get; set; }
    }

    public class CodeMatchesBySupplierId : Dictionary<string, CodeMatch>
    {

    }

    public class SingleDestinationCodeMatches
    {
        public string RouteCode { get; set; }

        public CodeMatchesBySupplierId CodeMatchesBySupplierId { get; set; }
    }
}
