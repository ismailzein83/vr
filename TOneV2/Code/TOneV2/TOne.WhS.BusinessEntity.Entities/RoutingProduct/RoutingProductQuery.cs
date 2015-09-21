using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductQuery
    {
        public string Name { get; set; }

        public List<int> SaleZonePackageIds { get; set; }
    }
}
