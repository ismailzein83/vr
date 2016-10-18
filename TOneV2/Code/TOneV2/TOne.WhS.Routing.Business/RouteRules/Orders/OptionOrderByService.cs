using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByService : RouteOptionOrderSettings
    {
        public override Guid ConfigId { get { return new Guid("E9519598-354C-41D0-BB89-5371F26D0A5D"); } }
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDitection = OrderDirection.Descending;
            foreach(IRouteOptionOrderTarget option in context.Options)
            {
                option.OptionWeight = option.SupplierServiceWeight;
            }
        }
    }
}
