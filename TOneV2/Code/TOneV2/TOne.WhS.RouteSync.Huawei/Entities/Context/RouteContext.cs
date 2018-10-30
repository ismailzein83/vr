using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public interface IRouteInitializeContext
    {

    }

    public class RouteInitializeContext : IRouteInitializeContext
    {

    }

    public interface IRouteCompareTablesContext
    {
        Dictionary<int, HuaweiConvertedRouteDifferences> RouteDifferencesByRSSN { set; }
    }

    public class RouteCompareTablesContext : IRouteCompareTablesContext
    {
        public Dictionary<int, HuaweiConvertedRouteDifferences> RouteDifferencesByRSSN { get; set; }
    }

    public interface IRouteFinalizeContext
    {

    }

    public class RouteFinalizeContext : IRouteFinalizeContext
    {

    }
}
