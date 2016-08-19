using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingCustomerInfo
    {
        public int CustomerId { get; set; }
    }

    public class RoutingCustomerInfoDetails
    {
        public int CustomerId { get; set; }

        public int SellingProductId { get; set; }
    }
}
