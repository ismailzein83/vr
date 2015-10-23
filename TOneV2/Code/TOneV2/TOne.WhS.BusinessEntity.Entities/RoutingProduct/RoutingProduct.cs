using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProduct
    {
        public int RoutingProductId { get; set; }

        public string Name { get; set; }

        public int SellingNumberPlanId { get; set; }

        public RoutingProductSettings Settings { get; set; }
    }
}
