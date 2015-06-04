using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCR.Entities
{
    /// <summary>
    /// Key is Supplier Zone ID, Value is CodeGroupSupplierZones
    /// </summary>
    public class SupplierZones : Dictionary<int, SupplierZoneInfo>
    {
        
    }

    public class SupplierZoneInfo
    {
        public bool IsCodeGroup { get; set; }

        public string SupplierId { get; set; }
    }
}
