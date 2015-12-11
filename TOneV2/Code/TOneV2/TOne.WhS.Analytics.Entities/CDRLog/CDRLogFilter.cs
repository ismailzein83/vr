using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.Analytics.Entities
{
    public class CDRLogFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<int> CustomerIds { get; set; }
        public List<int> SupplierIds { get; set; }
        public List<long> SaleZoneIds { get; set; }
        public List<long> SupplierZoneIds { get; set; } 
        
    }
}
