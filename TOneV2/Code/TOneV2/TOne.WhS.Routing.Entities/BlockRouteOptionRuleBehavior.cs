using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BlockRouteOptionRuleBehavior
    {
        public abstract BlockRouteOptionRuleBehaviorOutput Execute(BusinessEntity.Entities.BlockRouteOptionRule ruleDefinition);
    }

    public class BlockRouteOptionRuleBehaviorOutput
    {
        public List<SupplierWithZones> SuppliersWithZones { get; set; }
    }

    public class SupplierWithZones
    {
        public int SupplierId { get; set; }

        public List<long> SupplierZones { get; set; }
    }
}
