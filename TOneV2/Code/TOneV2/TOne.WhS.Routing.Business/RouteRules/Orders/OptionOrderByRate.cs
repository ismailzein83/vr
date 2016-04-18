﻿using System;
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
            context.OrderDitection = OrderDirection.Ascending;
            foreach(IRouteOptionOrderTarget option in context.Options)
            {
                option.OptionWeight = option.SupplierRate;
            }
        }
    }
}
