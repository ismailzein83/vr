﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class RouteOptionBlockAction : BaseRouteOptionRuleAction
    {
        public MultipleSelection<string> Customers { get; set; }
    }
}
