using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleQuery
    {
        //public int? RoutingProductId { get; set; }

        public string Code { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<long> SaleZoneIds { get; set; }
    }
}
