using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRoutesBatch
    {
        public CustomerRoutesBatch()
        {
            this.CustomerRoutes = new List<CustomerRoute>();
        }
        
        public List<CustomerRoute> CustomerRoutes { get; set; }

    }
}
