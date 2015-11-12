using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerSellingProductDetail
    {

        public CustomerSellingProduct Entity { get; set; }
        public string CustomerName { get; set; }
        public string SellingProductName { get; set; }
    }
}
