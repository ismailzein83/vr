using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierZoneSummaryGenericFilter
    {
        public List<string> CustomerIds { get; set; }
        public List<string> SupplierIds { get; set; }
        public List<int> ZoneIds { get; set; }
    }
}
