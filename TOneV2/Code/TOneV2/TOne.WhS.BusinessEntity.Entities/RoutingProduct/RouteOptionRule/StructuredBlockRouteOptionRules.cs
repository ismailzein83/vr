using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StructuredBlockRouteOptionRules
    {
        public Dictionary<int, SupplierBlockRouteOptionRules> RulesBySupplier { get; set; }
    }

    public class SupplierBlockRouteOptionRules
    {
        public StructuredRouteRules<BlockRouteOptionRule> AllSupplierZonesRules { get; set; }

        public Dictionary<long, StructuredRouteRules<BlockRouteOptionRule>> RulesBySupplierZones { get; set; }
    }
}
