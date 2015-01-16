using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public class CodeMatch
    {
        public string Code { get; set; }
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public long SupplierCodeId { get; set; }
        public int SupplierZoneId { get; set; }
        public bool IsFuture { get; set; }
    }
}
