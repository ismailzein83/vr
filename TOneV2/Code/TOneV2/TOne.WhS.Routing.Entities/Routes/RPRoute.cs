﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRoute
    {
        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }

        public HashSet<int> SaleZoneServiceIds { get; set; }

        public bool IsBlocked { get; set; }

        public int ExecutedRuleId { get; set; }

        public Dictionary<int, RPRouteOptionSupplier> OptionsDetailsBySupplier { get; set; }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> RPOptionsByPolicy { get; set; }
    }
    public class RPRouteBatch
    {
        public RPRouteBatch()
        {
            this.RPRoutes = new List<RPRoute>();
        }
        public List<RPRoute> RPRoutes { get; set; }
    }
}
