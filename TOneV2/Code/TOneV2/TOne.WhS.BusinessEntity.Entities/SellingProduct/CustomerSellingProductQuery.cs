using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerSellingProductQuery
    {
        public List<int> CustomersIds{get;set;}
        public List<int> SellingProductsIds { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
