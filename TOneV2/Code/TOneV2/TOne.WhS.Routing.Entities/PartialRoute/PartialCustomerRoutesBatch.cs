using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class PartialCustomerRoutesBatch
    {
        public PartialCustomerRoutesBatch()
        {
            this.CustomerRoutes = new List<CustomerRoute>();
            this.AffectedCustomerRoutes = new Dictionary<int, CustomerRouteData>();
        }

        public List<CustomerRoute> CustomerRoutes { get; set; }

        public Dictionary<int, CustomerRouteData> AffectedCustomerRoutes { get; set; }
    }
}