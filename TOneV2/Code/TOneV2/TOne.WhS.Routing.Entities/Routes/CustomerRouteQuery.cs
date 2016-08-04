﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteQuery
    {
        public int RoutingDatabaseId { get; set; }

        public string Code { get; set; }

        public List<int> CustomerIds { get; set; }

        public int LimitResult { get; set; }
    }
}
