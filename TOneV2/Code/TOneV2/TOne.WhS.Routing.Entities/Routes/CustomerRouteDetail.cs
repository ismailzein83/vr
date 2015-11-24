﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteDetail
    {
        public CustomerRoute Entity { get; set; }

        public string CustomerName { get; set; }

        public string ZoneName { get; set; }

        public string RouteOptions { get; set; }
    }
}
