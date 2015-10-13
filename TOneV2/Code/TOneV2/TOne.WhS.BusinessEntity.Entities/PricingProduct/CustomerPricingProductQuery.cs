using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPricingProductQuery
    {
        public List<int> CustomersIds{get;set;}
        public List<int> PricingProductsIds { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
