using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteOverrideAction : BaseRouteRuleAction
    {
        public List<OverrideOption> Options { get; set; }

        public override string ActionDisplayName
        {
            get { return "Route Override"; }
        }

        public override RouteRuleExecutionResult Execute(RouteDetail route, BaseRouteRule ruleDefinition, SupplierZoneRates supplierZoneRates)
        {
            return base.Execute(route, ruleDefinition, supplierZoneRates);
        }
    }

    public class OverrideOption
    {
        public string SupplierId { get; set; }

        public Int16? Percentage { get; set; }
    }
}
