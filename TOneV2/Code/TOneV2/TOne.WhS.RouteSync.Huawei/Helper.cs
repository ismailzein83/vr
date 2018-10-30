using System.Collections.Generic;
using System.Text;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public static class Helper
    {
        public const string RouteCaseFieldsSeparatorAsString = "_";
        public const string RouteCaseOptionsSeparatorAsString = "|";
        public const string RouteCaseOptionsFieldsSeparatorAsString = "~";

        public static string GetRSName(RTANA rtana, int routeNameLength)
        {
            if (rtana == null || rtana.RouteCaseOptions == null || rtana.RouteCaseOptions.Count == 0)
                return "BLK";

            StringBuilder sb_RSName = new StringBuilder(rtana.RSSN);

            foreach (var routeCaseOption in rtana.RouteCaseOptions)
            {
                string percentageAsString = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;
                string routeName = routeCaseOption.RouteName.Replace(" ", "");
                string routeNameSymbole = routeName.Substring(0, routeNameLength);

                sb_RSName.AppendFormat("_{0}{1}", routeNameSymbole, percentageAsString);
            }

            return sb_RSName.ToString();
        }

        public static string SerializeRouteCase(RTANA rtana)
        {
            if (rtana == null || rtana.RouteCaseOptions == null || rtana.RouteCaseOptions.Count == 0)
                return "BLK";

            string isSequenceAsString = rtana.IsSequence ? "SEQ" : "PERC";
            string serializedRouteCaseOptions = SerializeRouteCaseOptions(rtana.RouteCaseOptions);

            return string.Format("{0}{1}{0}{2}{0}{3}", RouteCaseFieldsSeparatorAsString, rtana.RSSN, isSequenceAsString, serializedRouteCaseOptions);
        }

        private static string SerializeRouteCaseOptions(List<RouteCaseOption> routeCaseOptions)
        {
            if (routeCaseOptions == null || routeCaseOptions.Count == 0)
                return null;

            StringBuilder sb_RouteCaseOptions = new StringBuilder();

            foreach (var routeCaseOption in routeCaseOptions)
            {
                if (sb_RouteCaseOptions.Length > 0)
                    sb_RouteCaseOptions.Append(RouteCaseOptionsSeparatorAsString);

                string percentageAsString = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;

                sb_RouteCaseOptions.AppendFormat("{1}{0}{2}{0}{3}", RouteCaseOptionsFieldsSeparatorAsString, routeCaseOption.RouteName, percentageAsString, routeCaseOption.ISUP);
            }

            return sb_RouteCaseOptions.ToString();
        }
    }
}