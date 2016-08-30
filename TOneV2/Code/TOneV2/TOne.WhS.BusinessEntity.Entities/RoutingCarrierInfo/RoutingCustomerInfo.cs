using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingCustomerInfo : RoutingCarrierInfo
    {
        public int CustomerId { get; set; }

        public override int CarrierInfoId { get { return CustomerId; } }

        public override string Title { get { return "Customer"; } }
    }

    public class RoutingCustomerInfoDetails
    {
        public int CustomerId { get; set; }

        public int SellingProductId { get; set; }
    }
}
