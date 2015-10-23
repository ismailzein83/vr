using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProductDetail
    {
        public int SellingProductId { get; set; }

        public string Name { get; set; }

        public int SellingNumberPlanId { get; set; }
        public string SellingNumberPlanName { get; set; }

        public int? DefaultRoutingProductId { get; set; }
        public string DefaultRoutingProductName { get; set; }

        public SellingProductSettings Settings { get; set; }
    }
}
