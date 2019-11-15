using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteMarginStaging : BaseCustomerRouteMargin
    {
        public HashSet<string> Codes { get; set; }

        public CustomerRouteOptionMarginStaging CustomerRouteOptionMarginStaging { get; set; }

        public CustomerRouteOptionMarginStaging OptimalCustomerRouteOptionMarginStaging { get; set; }
    }
}