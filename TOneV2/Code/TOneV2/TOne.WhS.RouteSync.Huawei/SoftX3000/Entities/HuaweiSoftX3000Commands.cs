using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public static class HuaweiSoftX3000Commands
    {
        public const string SRST_SEQ = "SEQ";

        public const string SRST_PERC = "PERC";

        public const string ROUTE_BLOCK = ""; // Waiting Support Response

        public const string RT_SR = "SR{0}={1}";

        public const string RT_PERC = "PERC{0}={1}";

        #region Patterns

        public const string RouteId_Pattern = "#RouteId#";
        public const string RouteCaseOptionsType_Pattern = "#RouteCaseOptionsType#";
        public const string PERCCFG_Pattern = "#PERCCFG#";
        public const string RouteCaseId_Pattern = "#RouteCaseId#";
        public const string RSSC_Pattern = "#RSSC#";
        public const string MINL_Pattern = "#MINL#";
        public const string MAXL_Pattern = "#MAXL#";
        public const string CHSC_Pattern = "#CHSC#";
        public const string Code_Pattern = "#Code#";
        public const string DNSet_Pattern = "#DNSet#";
        public const string ZoneName_Pattern = "#ZoneName#";
        public const string AllSR_Pattern = "#List<SR>#";

        #endregion

        public static string Full_Add_RT
        {
            get { return $"ADD RT: R={RouteId_Pattern}, RN=\"{RouteId_Pattern}\", IDTP=UNKNOWN, NAMECFG=NO, SNCM=SRT, SRST={RouteCaseOptionsType_Pattern}, PERCCFG={PERCCFG_Pattern}, {AllSR_Pattern}, STTP=INVALID, REM=NO"; }
        }

        public static string Full_Modify_RT
        {
            get { return $"MOD RT: R={RouteId_Pattern}, NAMECFG=NO, SNCM=SRT, {AllSR_Pattern}"; }
        }

        public static string Full_ADD_RTANA
        {
            get { return $"ADD RTANA: RAN=\"{RouteId_Pattern}\", RSC={RouteCaseId_Pattern}, RSSC={RSSC_Pattern}, TM=TMM, R={RouteId_Pattern}"; }
        }

        public static string Full_ADD_CNACLD
        {
            get { return $"ADD CNACLD: LP={DNSet_Pattern}, PFX=K'{Code_Pattern}, CSTP=BASE, CSA=ITT, RSC={RouteCaseId_Pattern}, MINL={MINL_Pattern}, MAXL={MAXL_Pattern}, CHSC={CHSC_Pattern}, EA=YES, DEST={RouteCaseId_Pattern}, ASF=NO, QCDN=NO, OBDTMFFLAG=NO, SDESCRIPTION=\"{ZoneName_Pattern}\", SAF=NO;"; }
        }

        public const string OpenSSLSucceeded = ""; // Waiting Support Response

        public const string LoginSucceeded = ""; // Waiting Support Response

        public const string RegistrationSucceeded = ""; // Waiting Support Response

        public const string UnregistrationSucceeded = ""; // Waiting Support Response

        public const string LogoutSucceeded = ""; // Waiting Support Response

        public const string RouteOptionsAddedSuccessfully = ""; // Waiting Support Response

        public const string RouteOptionsExists = ""; // Waiting Support Response

        public const string RouteCaseAddedSuccessfully = ""; // Waiting Support Response

        public const string RouteCaseExists = ""; // Waiting Support Response

        public const string RouteOperationSucceeded = ""; // Waiting Support Response
    }

    public class RouteOptionsWithCommands
    {
        public RouteOptions RouteOptions { get; set; }
        public List<string> Commands { get; set; }
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