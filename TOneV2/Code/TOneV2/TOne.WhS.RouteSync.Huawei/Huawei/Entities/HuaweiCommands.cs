using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public static class HuaweiCommands
    {
        public const string RTSM_SEQ = "SEQ";

        public const string RTSM_PERC = "PERC";

        public const string ROUTE_BLOCK = "BLK";

        public const string CSA_ITT = "ITT";

        public const string CSA_TON = "TON";

        public const string OpenSSLSucceeded = "RETURN:0";

        public const string LoginSucceeded = "RETCODE=0";

        public const string RegistrationSucceeded = "RETCODE=0";

        public const string UnregistrationSucceeded = "RETCODE=0";

        public const string LogoutSucceeded = "RETCODE=0";

        public const string RouteCaseAddedSuccessfully = "RETCODE=0OPERATIONSUCCEEDED";

        public const string RouteCaseExists = "ROUTEANALYSISRECORDEXISTS";

        public const string RouteOperationSucceeded = "RETCODE=0OPERATIONSUCCEEDED";
    }

    public class RouteCaseWithCommands
    {
        public RouteCase RouteCase { get; set; }
        public List<string> Commands { get; set; }
    }

    public class HuaweiRouteWithCommands
    {
        public HuaweiConvertedRouteCompareResult RouteCompareResult { get; set; }
        public RouteActionType ActionType { get; set; }
        public List<string> Commands { get; set; }
    }
}