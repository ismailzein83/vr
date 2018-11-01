using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public static class HuaweiCommands
    {
        public const string RTSM_SEQ = "SEQ";

        public const string RTSM_PERC = "PERC";

        public const string ROUTE_BLOCK = "BLK";

        public const string CSA_ITT = "ITT";

        public const string CSA_TON = "TON";
    }

    public class RouteCaseWithCommands
    {
        public RouteCase RouteCase { get; set; }
        public List<string> Commands { get; set; }
    }

    public class HuaweiRouteWithCommands
    {
        public HuaweiConvertedRouteCompareResult RouteCompareResult { get; set; }
        public List<string> Commands { get; set; }
        public RouteActionType ActionType { get; set; }
    }
}