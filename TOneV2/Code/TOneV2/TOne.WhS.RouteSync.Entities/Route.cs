﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class Route
    {
        public string CustomerId { get; set; }

        public string Code { get; set; }

        public List<RouteOption> Options { get; set; }
    }
}
