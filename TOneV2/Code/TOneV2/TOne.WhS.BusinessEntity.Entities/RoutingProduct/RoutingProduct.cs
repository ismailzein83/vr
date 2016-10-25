using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseRoutingProduct
    {
        public int RoutingProductId { get; set; }

        public string Name { get; set; }

        public RoutingProductSettings Settings { get; set; }
    }

    public class RoutingProduct : BaseRoutingProduct
    {
        public int SellingNumberPlanId { get; set; }
    }

    public class RoutingProductToEdit : BaseRoutingProduct
    {

    }
}
