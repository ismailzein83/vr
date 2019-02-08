using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei
{
    public static class Helper
    {
        const string RouteCaseFieldsSeparatorAsString = "^$^$^";
        const string RouteCaseOptionsSeparatorAsString = "|@|@|";
        const string RouteCaseOptionsFieldsSeparatorAsString = "~%~%~";

        public static string GetRSName(RouteAnalysis routeAnalysis, int routeNameLength, Dictionary<string, string> overriddenRSSNsInRSName)
        {
            if (routeAnalysis == null || routeAnalysis.RouteCaseOptions == null || routeAnalysis.RouteCaseOptions.Count == 0)
                return HuaweiCommands.ROUTE_BLOCK;

            StringBuilder sb_RSName = new StringBuilder();

            string overriddenRSSNInRSName;
            if (overriddenRSSNsInRSName == null || !overriddenRSSNsInRSName.TryGetValue(routeAnalysis.RSSN, out overriddenRSSNInRSName))
                sb_RSName.Append(routeAnalysis.RSSN);
            else
                sb_RSName.Append(overriddenRSSNInRSName);

            foreach (var routeCaseOption in routeAnalysis.RouteCaseOptions)
            {
                string routeName = routeCaseOption.RouteName.Replace(" ", "");
                string routeNameSymbole = routeName.Substring(0, routeNameLength);
                string percentageAsString = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;

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

        public static RouteAnalysis DeserializeRouteCase(string routeCaseAsString, int rcNumber)
        {
            routeCaseAsString.ThrowIfNull("routeCaseAsString");

            string[] routeCaseFields = routeCaseAsString.Split(new string[] { RouteCaseFieldsSeparatorAsString }, StringSplitOptions.None);
            routeCaseFields.ThrowIfNull("routeCaseFields");

            if (routeCaseFields.Count() != 3)
                throw new Exception($"Invalid character in Route Case fields for RC#: '{rcNumber}'");

            RouteAnalysis routeAnalysis = new RouteAnalysis();
            routeAnalysis.RSSN = routeCaseFields[0];

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