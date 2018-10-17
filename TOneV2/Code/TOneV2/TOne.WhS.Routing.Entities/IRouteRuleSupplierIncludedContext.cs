﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleIsSupplierIncludedContext
    {
        IEnumerable<int> SupplierIds { get; }
    }
    public class RouteRuleIsSupplierIncludedContext : IRouteRuleIsSupplierIncludedContext
    {
        public IEnumerable<int> SupplierIds { get; set; }
    }
}
