using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public class OptionFilterLoss : RouteOptionFilterSettings
    {
        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            if (context.SaleRate.HasValue && context.SaleRate.Value < context.Option.SupplierRate)
                context.FilterOption = true;
        }
    }
}
