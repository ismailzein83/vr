using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000
{
    public static class Helper
    {
        const string RouteOptionsSeparator = "##";
        const string PercentageSeparator = "%%";
        public static string SerializeRouteAnalysis(RouteAnalysis routeAnalysis)
        {
            if (routeAnalysis == null || routeAnalysis.RouteCaseOptions == null || routeAnalysis.RouteCaseOptions.Count == 0)
                return HuaweiSoftX3000Commands.ROUTE_BLOCK;

            StringBuilder sb_routeOptionsName = new StringBuilder();
            List<string> backupRouteOptionsNameHS = new List<string>();
            foreach (var routeCaseOption in routeAnalysis.RouteCaseOptions)
            {
                string stringSRT = routeCaseOption.SRT.ToString();

                switch (routeAnalysis.RouteOptionsType)
                {
                    case RouteCaseOptionsType.Percentage:
                        {
                            if (routeCaseOption.Percentage.HasValue)
                            {
                                sb_routeOptionsName.Append($"{stringSRT}{PercentageSeparator}{routeCaseOption.Percentage.Value}");
                            }
                            else
                            {
                                backupRouteOptionsNameHS.Add(stringSRT);
                                continue;
                            }
                            break;
                        }
                    case RouteCaseOptionsType.Sequence:
                        {
                            sb_routeOptionsName.Append(stringSRT);
                            break;
                        }

                    default: throw new NotSupportedException($"RouteCaseOptionsType: {routeAnalysis.RouteOptionsType}");
                }

                sb_routeOptionsName.Append(RouteOptionsSeparator);
            }

            if (backupRouteOptionsNameHS.Count > 0)
                sb_routeOptionsName.Append(string.Join(RouteOptionsSeparator, backupRouteOptionsNameHS));
            else
                sb_routeOptionsName.Remove(sb_routeOptionsName.Length - RouteOptionsSeparator.Length, RouteOptionsSeparator.Length);

            return sb_routeOptionsName.ToString();
        }

        public static List<RouteCaseOption> DeserializeRouteOptions(string routeOptionsAsString, out bool isSequence)
        {
            routeOptionsAsString.ThrowIfNull("routeOptionsAsString");

            isSequence = !routeOptionsAsString.Contains(PercentageSeparator);

            List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();

            string[] routeOptionsArray = routeOptionsAsString.Split(new string[] { RouteOptionsSeparator }, StringSplitOptions.None);

            if (routeOptionsArray.Length == 0)
                throw new Exception($"Invalid Route Options '{routeOptionsAsString}'");

            for (int i = 0; i < routeOptionsArray.Length; i++)
            {
                string routeOptionString = routeOptionsArray[i];
                RouteCaseOption routeCaseOption = new RouteCaseOption();

                int SRT;
                int? percentage = null;
                if (!isSequence)
                {
                    string[] routeOptionFieldsArray = routeOptionString.Split(new string[] { PercentageSeparator }, StringSplitOptions.None);

                    if (!int.TryParse(routeOptionFieldsArray[0], out SRT))
                        throw new Exception($"Invalid Route Option format '{routeOptionFieldsArray[0]}' ");

                    if (routeOptionFieldsArray.Length == 2)
                    {
                        if (!int.TryParse(routeOptionFieldsArray[1], out int perc))
                            throw new Exception($"Invalid Percentage '{routeOptionFieldsArray[1]}' for Route Option '{routeOptionFieldsArray[0]}' and SRT '{SRT}'");
                        else
                            percentage = perc;
                    }
                }
                else if (!int.TryParse(routeOptionString, out SRT))
                {
                    throw new Exception($"Invalid Route Option format: {routeOptionString}");
                }

                routeCaseOptions.Add(new RouteCaseOption()
                {
                    SRT = SRT,
                    Percentage = percentage
                });
            }

            return routeCaseOptions;
        }

        public static bool RAN_RSSCExists(Dictionary<string, Dictionary<int, RouteCase>> routeCasesByRSSCByRAN, string RAN, int RSSC)
        {
            routeCasesByRSSCByRAN.ThrowIfNull("routeCasesByRSSCByRAN");
            return routeCasesByRSSCByRAN.TryGetValue(RAN, out Dictionary<int, RouteCase> routeCasesByRSSC) && routeCasesByRSSC != null && routeCasesByRSSC.ContainsKey(RSSC);
        }

        public static RouteCase GetRouteCaseByRANThenRSSC(Dictionary<string, Dictionary<int, RouteCase>> routeCasesByRSSCByRAN, string RAN, int RSSC)
        {
            routeCasesByRSSCByRAN.ThrowIfNull("routeCasesByRSSCByRAN");
            if (routeCasesByRSSCByRAN.TryGetValue(RAN, out Dictionary<int, RouteCase> routeCasesByRSSC) && routeCasesByRSSC != null && routeCasesByRSSC.TryGetValue(RSSC, out RouteCase routeCase))
                return routeCase;

            return null;
        }
    }
}
