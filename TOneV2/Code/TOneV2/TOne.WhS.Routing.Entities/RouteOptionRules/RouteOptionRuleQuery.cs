using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleQuery
    {
        public string Name { get; set; }

        public int? RoutingProductId { get; set; }

        public string Code { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<long> SaleZoneIds { get; set; }
        public int? SupplierId { get; set; }
        public IEnumerable<long> SupplierZoneIds { get; set; }
        public DateTime? EffectiveOn { get; set; }

        public List<Guid> RouteOptionRuleSettingsConfigIds { get; set; }

        public List<int> LinkedRouteOptionRuleIds { get; set; }
    }
}
