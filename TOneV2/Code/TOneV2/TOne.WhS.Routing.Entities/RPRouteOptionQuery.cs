﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionQuery
    {
        public int RoutingDatabaseId { get; set; }

        public int PolicyOptionConfigId { get; set; }

        public int RoutingProductId { get; set; }

        public int SaleZoneId { get; set; }
    }
}
