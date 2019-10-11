using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public interface IRouteInitializeContext
    {
    }

    public class RouteInitializeContext : IRouteInitializeContext
    {
    }

    public interface IRouteCompareTablesContext
    {
        Dictionary<int, HuaweiConvertedRouteDifferences> RouteDifferencesByRSSC { set; }
    }

    public class RouteCompareTablesContext : IRouteCompareTablesContext
    {
        public Dictionary<int, HuaweiConvertedRouteDifferences> RouteDifferencesByRSSC { get; set; }
    }

    public interface IRouteFinalizeContext
    {
    }

    public class RouteFinalizeContext : IRouteFinalizeContext
    {
    }
}
