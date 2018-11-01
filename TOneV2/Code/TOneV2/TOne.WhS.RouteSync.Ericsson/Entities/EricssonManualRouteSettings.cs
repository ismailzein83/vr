﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class EricssonManualRouteSettings
    {
        public List<EricssonManualRoute> EricssonManualRoutes { get; set; }
    }

    public class EricssonManualRoute
    {
        public List<string> Customers { get; set; }
        public EricssonManualRouteOriginations ManualRouteOriginations { get; set; }
        public EricssonManualRouteDestinations ManualRouteDestinations { get; set; }
        public EricssonManualRouteAction ManualRouteAction { get; set; }
    }
}
