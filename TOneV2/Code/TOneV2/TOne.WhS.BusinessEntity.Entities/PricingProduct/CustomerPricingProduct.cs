using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPricingProduct
    {
        public int CustomerPricingProductId { get; set; }
        public int CustomerId { get; set; }
        public int PricingProductId{get;set;}
        public bool AllDestinations { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
