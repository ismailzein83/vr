using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiConvertedRoute : ConvertedRouteWithCode
    {
        public string RSSN { get; set; }
        public string RSName { get; set; }
        public int DNSet { get; set; }

        public override string GetCustomer()
        {
            return this.RSSN;
        }

        public override string GetRouteOptionsIdentifier()
        {
            return this.RSName;
        }
    }

    public class HuaweiConvertedRouteCompareResult
    {
        public HuaweiConvertedRoute Route { get; set; }
        public HuaweiConvertedRoute ExistingRoute { get; set; }
    }

    public class HuaweiConvertedRouteDifferences
    {
        public HuaweiConvertedRouteDifferences()
        {
            RoutesToAdd = new List<HuaweiConvertedRouteCompareResult>();
            RoutesToUpdate = new List<HuaweiConvertedRouteCompareResult>();
            RoutesToDelete = new List<HuaweiConvertedRouteCompareResult>();
        }

        public List<HuaweiConvertedRouteCompareResult> RoutesToAdd { get; set; }
        public List<HuaweiConvertedRouteCompareResult> RoutesToUpdate { get; set; }
        public List<HuaweiConvertedRouteCompareResult> RoutesToDelete { get; set; }
    }
     
    public struct HuaweiConvertedRouteIdentifier
    {
        public string RSSN { get; set; }
        public string Code { get; set; }
        public int DNSet { get; set; }

        public override int GetHashCode()
        {
            return RSSN.GetHashCode() + Code.GetHashCode() + DNSet.GetHashCode();
        }
    }
}