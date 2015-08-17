using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class GenericFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<string> PortIn { get; set; }
        public List<string> PortOut { get; set; }
        public List<string> GateWayIn { get; set; }
        public List<string> GateWayOut { get; set; }
        public List<string> CustomerIds { get; set; }
        public List<string> CodeSales { get; set; }
        public List<string> CodeBuy { get; set; }
        public List<string> SupplierIds { get; set; }
        public List<int> SupplierZoneId { get; set; }

        public List<string> CodeGroups { get; set; }

        public List<int> ZoneIds { get; set; }
    }
}
