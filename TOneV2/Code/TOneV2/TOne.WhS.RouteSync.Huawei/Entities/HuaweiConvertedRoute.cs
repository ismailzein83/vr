using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiConvertedRoute : ConvertedRouteWithCode
    {
        public int RSSN { get; set; }
        public string RSName { get; set; }
        public int DNSet { get; set; }

        public override string GetCustomer()
        {
            return this.RSSN.ToString();
        }

        public override string GetRouteOptionsIdentifier()
        {
            return this.RSName;
        }
    }

    public class HuaweiConvertedRouteCompareResult
    {
        public HuaweiConvertedRoute NewRoute { get; set; }
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
}