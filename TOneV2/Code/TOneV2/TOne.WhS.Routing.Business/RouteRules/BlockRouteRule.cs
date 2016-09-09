using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class BlockRouteRule : RouteRuleSettings
    {
        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            target.BlockRoute = true;
        }

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            target.BlockRoute = true;
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
        }
    }



    public class RoutingOptimizationOptionPercentage : RouteOptionPercentageSettings
    {
        public List<RoutingOptimizationOptionPercentageItem> Items { get; set; }
        public override void Execute(IRouteOptionPercentageExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }


    public class RoutingOptimizationOptionPercentageItem
    {
        public int RoutingOptimizerItemConfigId { get; set; }

        public int PercentageFactor { get; set; }
    }
}
