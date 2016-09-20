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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("0ee52656-1bab-4061-8b43-f75ee38bc55e"); } }
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDitection = OrderDirection.Ascending;
            foreach(IRouteOptionOrderTarget option in context.Options)
            {
                option.OptionWeight = option.SupplierRate;
            }
        }
    }
}
