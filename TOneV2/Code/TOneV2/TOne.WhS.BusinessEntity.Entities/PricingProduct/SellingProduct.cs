using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProduct
    {
        public int SellingProductId { get; set; }

        public string Name { get; set; }

        public int SellingNumberPlanId { get; set; }

        public int? DefaultRoutingProductId { get; set; }

        public SellingProductSettings Settings { get; set; }
    }
}
