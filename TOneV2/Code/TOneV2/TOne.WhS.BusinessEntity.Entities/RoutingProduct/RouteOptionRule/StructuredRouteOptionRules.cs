using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StructuredRouteOptionRules
    {
        public Dictionary<int, SupplierRouteOptionRules> RulesBySupplier { get; set; }
    }

    public class SupplierRouteOptionRules
    {
        public StructuredRouteRules<RouteOptionRule> AllSupplierZonesRules { get; set; }

        public Dictionary<long, StructuredRouteRules<RouteOptionRule>> RulesBySupplierZones { get; set; }
    }
}
