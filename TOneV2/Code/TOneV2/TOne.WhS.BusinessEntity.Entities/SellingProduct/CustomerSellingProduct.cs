using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerSellingProduct
    {
        public int CustomerSellingProductId { get; set; }
        public int CustomerId { get; set; }
        public int SellingProductId { get; set; }
        public DateTime BED { get; set; }
    }
}
