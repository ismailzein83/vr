using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByQuality : RouteOptionOrderSettings
    {
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDitection = OrderDirection.Descending;
            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                Random rand = new Random();
                option.OptionWeight = Convert.ToDecimal(rand.NextDouble() * 1000);
            }
        }
    }
}
