using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum RoutingEntityType { PartialRouteInfo = 0, BERoute = 1 }

    public class RoutingEntityDetails
    {
        public RoutingEntityType RoutingEntityType { get; set; }
        public RoutingEntityInfo RoutingEntityInfo { get; set; }
    }

    public abstract class RoutingEntityInfo
    {

    }

    public class PartialRouteInfo : RoutingEntityInfo
    {
        public int LastVersionNumber { get; set; }

        public RateRouteInfo SaleRateRouteInfo { get; set; }

        public RateRouteInfo SupplierRateRouteInfo { get; set; }

        public DateTime LatestRoutingDate { get; set; }

        public DateTime? NextOpenOrCloseRuleTime { get; set; }
    }

    public class RateRouteInfo
    {
        public int LatestVersionNumber { get; set; }
        public DateTime? NextOpenOrCloseRateTime { get; set; }
        public object MaxRateTimeStamp { get; set; }
    }

    public class BERouteInfo : RoutingEntityInfo
    {
        public int LatestSaleRateVersionNumber { get; set; }

        public int LatestCostRateVersionNumber { get; set; }
    }
}
