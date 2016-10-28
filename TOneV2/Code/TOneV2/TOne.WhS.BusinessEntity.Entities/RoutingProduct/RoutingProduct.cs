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

    public class RoutingProductEditorRuntime
    {
        public RoutingProduct Entity { get; set; }

        public Dictionary<long, string> ZoneNames { get; set; }

        public Dictionary<int, string> ServiceNames { get; set; }
    }

    public class RoutingProductToEdit : BaseRoutingProduct
    {

    }
}
