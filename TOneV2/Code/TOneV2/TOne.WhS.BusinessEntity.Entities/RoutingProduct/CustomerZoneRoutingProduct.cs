using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CustomerZoneRoutingProductSource { CustomerZone, CustomerDefault, ProductZone, ProductDefault }
    public class CustomerZoneRoutingProduct
    {
        public int RoutingProductId { get; set; }

        public CustomerZoneRoutingProductSource Source { get; set; }
    }
}
