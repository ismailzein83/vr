using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public static class Helper
    {
        public const string RouteCaseFieldsSeparatorAsString = "_";
        public const string RouteCaseOptionsSeparatorAsString = "|";
        public const string RouteCaseOptionsFieldsSeparatorAsString = "~";

        public static string GetRSName(RouteAnalysis routeAnalysis, int routeNameLength)
        {
            if (routeAnalysis == null || routeAnalysis.RouteCaseOptions == null || routeAnalysis.RouteCaseOptions.Count == 0)
                return HuaweiCommands.ROUTE_BLOCK;

            StringBuilder sb_RSName = new StringBuilder(routeAnalysis.RSSN.ToString());

            foreach (var routeCaseOption in routeAnalysis.RouteCaseOptions)
            {
                string percentageAsString = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;
                string routeName = routeCaseOption.RouteName.Replace(" ", "");
                string routeNameSymbole = routeName.Substring(0, routeNameLength);

                sb_RSName.AppendFormat("_{0}{1}", routeNameSymbole, percentageAsString);
            }

            return sb_RSName.ToString();
        }

        public static string SerializeRouteCase(RouteAnalysis routeAnalysis)
        {
            if (routeAnalysis == null || routeAnalysis.RouteCaseOptions == null || !routeAnalysis.RouteCaseOptions.Any())
                return HuaweiCommands.ROUTE_BLOCK;

            string routeCaseOptionsTypeAsString;
            switch (routeAnalysis.RouteCaseOptionsType)
            {
                case RouteCaseOptionsType.Sequence: routeCaseOptionsTypeAsString = HuaweiCommands.RTSM_SEQ; break;
                case RouteCaseOptionsType.Percentage: routeCaseOptionsTypeAsString = HuaweiCommands.RTSM_PERC; break;
                default: throw new NotSupportedException(string.Format("rtana.RouteCaseOptionsType {0} not supported", routeAnalysis.RouteCaseOptionsType));
            }

            string serializedRouteCaseOptions = SerializeRouteCaseOptions(routeAnalysis.RouteCaseOptions);

            return string.Format("{1}{0}{2}{0}{3}", RouteCaseFieldsSeparatorAsString, routeAnalysis.RSSN, routeCaseOptionsTypeAsString, serializedRouteCaseOptions);
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

        public static RouteAnalysis DeserializeRouteCase(string routeCaseAsString)
        {
            if (string.IsNullOrEmpty(routeCaseAsString))
                return null;

            string[] routeCaseFields = routeCaseAsString.Split(new string[] { RouteCaseFieldsSeparatorAsString }, StringSplitOptions.None);
            if (routeCaseFields == null || routeCaseFields.Count() != 3)
                return null;

            RouteAnalysis routeAnalysis = new RouteAnalysis();
            routeAnalysis.RSSN = int.Parse(routeCaseFields[0]);

            string routeCaseOptionsTypeAsString = routeCaseFields[1];
            switch (routeCaseOptionsTypeAsString)
            {
                case HuaweiCommands.RTSM_SEQ: routeAnalysis.RouteCaseOptionsType = RouteCaseOptionsType.Sequence; break;
                case HuaweiCommands.RTSM_PERC: routeAnalysis.RouteCaseOptionsType = RouteCaseOptionsType.Percentage; break;
                default: throw new NotSupportedException(string.Format("rtana.RouteCaseOptionsType {0} not supported", routeAnalysis.RouteCaseOptionsType));
            }

            routeAnalysis.RouteCaseOptions = DeserializeRouteCaseOptions(routeCaseFields[2]);
            return routeAnalysis;
        }

        public static List<RouteCaseOption> DeserializeRouteCaseOptions(string serializedRouteCaseOption)
        {
            if (string.IsNullOrEmpty(serializedRouteCaseOption))
                return null;

            string[] routeCaseOptions = serializedRouteCaseOption.Split(new string[] { RouteCaseOptionsSeparatorAsString }, StringSplitOptions.None);
            if (routeCaseOptions == null || !routeCaseOptions.Any())
                return null;

            List<RouteCaseOption> results = new List<RouteCaseOption>();

            foreach (var routeCaseOption in routeCaseOptions)
            {
                string[] routeCaseOptionFields = routeCaseOption.Split(new string[] { RouteCaseOptionsFieldsSeparatorAsString }, StringSplitOptions.None);
                if (routeCaseOptionFields == null || routeCaseOptionFields.Count() != 3)
                    continue;

                int? percentage = null;
                if (!string.IsNullOrEmpty(routeCaseOptionFields[1]))
                    percentage = int.Parse(routeCaseOptionFields[1]);

                results.Add(new RouteCaseOption() { RouteName = routeCaseOptionFields[0], Percentage = percentage, ISUP = routeCaseOptionFields[2] });
            }

            return results.Count > 0 ? results : null;
        }
    }
}