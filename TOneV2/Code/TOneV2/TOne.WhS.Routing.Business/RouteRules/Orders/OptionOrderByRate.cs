using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByRate : RouteOptionOrderSettings
    {
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.Options = context.Options.OrderBy(itm => itm.SupplierRate);
        }
    }
}
