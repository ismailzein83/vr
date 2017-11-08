using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRoute : BaseRoute
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string SaleZoneName { get; set; }

        public int VersionNumber { get; set; }
    }

    public class CustomerRouteDefinition
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
    }
}
