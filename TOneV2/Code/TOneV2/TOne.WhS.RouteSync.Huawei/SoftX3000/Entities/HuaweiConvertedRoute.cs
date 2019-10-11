using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class HuaweiConvertedRoute : ConvertedRouteWithCode
    {
        public int DNSet { get; set; }
        public long RouteCaseId { get; set; }
        public string RAN { get; set; }
        public int RSSC { get; set; }
        public int MinL { get; set; }
        public int MaxL { get; set; }
        public int CC { get; set; }

        public override string GetCustomer()
        {
            return RSSC.ToString();
        }

        public override string GetRouteOptionsIdentifier()
        {
            return RouteCaseId.ToString();
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
        public int RSSC { get; set; }
        public string Code { get; set; }
        public int DNSet { get; set; }

        public override int GetHashCode()
        {
            return RSSC.GetHashCode() + Code.GetHashCode() + DNSet.GetHashCode();
        }
    }
}