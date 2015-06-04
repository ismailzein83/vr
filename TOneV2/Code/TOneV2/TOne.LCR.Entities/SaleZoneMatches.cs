using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    /// <summary>
    /// Key is Sale Zone Id
    /// </summary>
    public class SaleZoneMatches : Dictionary<int, SupplierZones>
    {
    }

    public class SaleZoneInfo
    {

        public int SaleZoneId { get; set; }
        public bool IsCodeGroup { get; set; }

    }
}
