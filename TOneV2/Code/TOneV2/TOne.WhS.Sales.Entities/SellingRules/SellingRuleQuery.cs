using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleQuery
    {
        public int? SellingProductId { get; set; }
        public IEnumerable<int> CustomerIds { get; set; }
        public IEnumerable<long> SaleZoneIds { get; set; }
    }
}
