﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionSettingsGroup
    {
        public int ConfigId { get; set; }

        public abstract IEnumerable<RouteOptionSettings> GetOptionSettings(RouteOptionSettingsContext context);
    }
}
