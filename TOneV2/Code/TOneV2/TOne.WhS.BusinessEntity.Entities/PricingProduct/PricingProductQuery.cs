using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingProductQuery
    {
        public string Name { get; set; }
        public List<int> SaleZonePackagesIds { get; set; }

        public List<int?> RoutingProductsIds { get; set; }
    }
}
