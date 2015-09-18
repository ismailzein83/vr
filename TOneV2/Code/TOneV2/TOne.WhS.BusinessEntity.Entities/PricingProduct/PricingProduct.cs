using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingProduct
    {
        public int PricingProductId { get; set; }

        public string Name { get; set; }

        public int SaleZonePackageId { get; set; }

        public int DefaultRoutingProductId { get; set; }

        public PricingProductSettings Settings { get; set; }
    }
}
