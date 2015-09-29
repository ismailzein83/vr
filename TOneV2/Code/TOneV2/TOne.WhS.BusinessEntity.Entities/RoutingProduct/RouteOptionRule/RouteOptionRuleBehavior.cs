using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class RouteOptionRuleBehavior
    {
        public abstract RouteOptionRuleBehaviorOutput Evaluate(BusinessEntity.Entities.RouteOptionRule ruleDefinition);
    }

    public class RouteOptionRuleBehaviorOutput
    {
        public List<SupplierWithZones> SuppliersWithZones { get; set; }
    }

    public class SupplierWithZones
    {
        public int SupplierId { get; set; }

        public List<long> SupplierZones { get; set; }
    }
}
