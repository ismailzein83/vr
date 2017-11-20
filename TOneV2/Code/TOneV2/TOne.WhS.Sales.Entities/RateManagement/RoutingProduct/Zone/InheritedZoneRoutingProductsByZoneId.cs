using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class InheritedZoneRoutingProductsByZoneId : Dictionary<long, ZoneInheritedRoutingProducts>
    {

    }
    //public class ZoneInheritedRoutingProducts
    //{
    //    public List<SaleZoneRoutingProduct> RoutingProducts { get; set; }
    //    public ZoneInheritedRoutingProducts()
    //    {
    //        RoutingProducts = new List<SaleZoneRoutingProduct>();
    //    }
    //}
}
