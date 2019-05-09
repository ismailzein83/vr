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
        public override Guid ConfigId { get { return new Guid("0ee52656-1bab-4061-8b43-f75ee38bc55e"); } }
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDirection = OrderDirection.Ascending;
            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                option.OptionWeight = option.SupplierRate.HasValue ? option.SupplierRate.Value : decimal.MaxValue;
            }
        }
    }
}
