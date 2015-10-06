using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPricingProductQuery
    {
        public int? CustomerId{get;set;}
        public int? PricingProductId { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
