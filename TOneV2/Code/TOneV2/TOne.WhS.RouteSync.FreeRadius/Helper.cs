using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public static class Helper
    {
        public static void AddToConvertedRoutesDict(Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit> convertedRoutesDict, List<FreeRadiusConvertedRoute> convertedRoutes)
        {
            if (convertedRoutesDict == null)
                return;

            foreach (var convertedRoute in convertedRoutes)
            {
                string code = convertedRoute.Cldsid;
                string codeWithoutLastDigit = code.Length > 1 ? convertedRoute.Cldsid.Substring(0, code.Length - 1) : string.Empty;
                string codeLastDigit = code.Substring(code.Length - 1, 1);
                OptionWithPercentage option = new OptionWithPercentage() { Option = convertedRoute.Option, Min_perc = convertedRoute.Min_perc, Max_perc = convertedRoute.Max_perc };
                //string optionAsString = string.Format("{0}~{1}~{2}", convertedRoute.Code, convertedRoute.Min_perc, convertedRoute.Max_perc);

                ConvertedRoutesByCodeWithoutLastDigit convertedRoutesByCodeWithoutLastDigit = convertedRoutesDict.GetOrCreateItem(convertedRoute.Customer_id);
                ConvertedRoutesByOption convertedRoutesByOption = convertedRoutesByCodeWithoutLastDigit.GetOrCreateItem(codeWithoutLastDigit);

                ConvertedRouteWithCodeLastDigit convertedRouteWithCodeLastDigit = new ConvertedRouteWithCodeLastDigit() { CodeLastDigit = codeLastDigit, FreeRadiusConvertedRoute = convertedRoute };

                ConvertedRoutesWithCodeLastDigit convertedRoutesWithCodeLastDigit;
                if (convertedRoutesByOption.TryGetValue(option, out convertedRoutesWithCodeLastDigit))
                {
                    convertedRoutesWithCodeLastDigit.ConvertedRouteWithCodeLastDigitList.Add(convertedRouteWithCodeLastDigit);
                }
                else
                {
                    convertedRoutesByOption.Add(option, new ConvertedRoutesWithCodeLastDigit()
                    {
                        FreeRadiusConvertedRouteOption = new FreeRadiusConvertedRouteOption() { Option = convertedRoute.Option, Min_perc = convertedRoute.Min_perc, Max_perc = convertedRoute.Max_perc },
                        ConvertedRouteWithCodeLastDigitList = new List<ConvertedRouteWithCodeLastDigit>() { convertedRouteWithCodeLastDigit }
                    });
                }
            }
        }

        public static List<FreeRadiusConvertedRoute> CompressConvertedRoutes(Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit> convertedRoutesDict)
        {
            if (convertedRoutesDict == null || convertedRoutesDict.Count == 0)
                return null;

            List<FreeRadiusConvertedRoute> freeRadiusConvertedRoute = new List<FreeRadiusConvertedRoute>();

            foreach (var kvp in convertedRoutesDict)
            {
                string customerId = kvp.Key;
                ConvertedRoutesByCodeWithoutLastDigit convertedRoutesByCodeWithoutLastDigit = kvp.Value;

                foreach (var convertedRouteByCode in convertedRoutesByCodeWithoutLastDigit)
                {
                    string codeWithoutLastDigit = convertedRouteByCode.Key;
                    ConvertedRoutesByOption convertedRoutesByOption = convertedRouteByCode.Value;

                    foreach (var convertedRouteByOption in convertedRoutesByOption)
                    {
                        ConvertedRoutesWithCodeLastDigit convertedRoutesWithCodeLastDigit = convertedRouteByOption.Value;
                        FreeRadiusConvertedRouteOption freeRadiusConvertedRouteOption = convertedRoutesWithCodeLastDigit.FreeRadiusConvertedRouteOption;

                        string minCodeLastDigit = null;
                        ConvertedRouteWithCodeLastDigit previousConvertedRouteWithCodeLastDigit = null;

                        foreach (var convertedRouteWithCodeLastDigit in convertedRoutesWithCodeLastDigit.ConvertedRouteWithCodeLastDigitList.OrderBy(itm => itm.CodeLastDigit))
                        {
                            if (previousConvertedRouteWithCodeLastDigit == null)
                            {
                                minCodeLastDigit = convertedRouteWithCodeLastDigit.CodeLastDigit;
                                previousConvertedRouteWithCodeLastDigit = convertedRouteWithCodeLastDigit;
                                continue;
                            }

                            if (!AreConsecutives(previousConvertedRouteWithCodeLastDigit.CodeLastDigit, convertedRouteWithCodeLastDigit.CodeLastDigit))
                            {
                                if (string.Compare(minCodeLastDigit, previousConvertedRouteWithCodeLastDigit.CodeLastDigit) != 0)
                                {
                                    freeRadiusConvertedRoute.Add(new FreeRadiusConvertedRoute()
                                    {
                                        Customer_id = customerId,
                                        Clisis = "''",
                                        Cldsid = string.Format("{0}[{1}-{2}]", codeWithoutLastDigit, minCodeLastDigit, previousConvertedRouteWithCodeLastDigit.CodeLastDigit),
                                        Option = freeRadiusConvertedRouteOption.Option,
                                        Min_perc = freeRadiusConvertedRouteOption.Min_perc,
                                        Max_perc = freeRadiusConvertedRouteOption.Max_perc
                                    });
                                }
                                else
                                {
                                    freeRadiusConvertedRoute.Add(previousConvertedRouteWithCodeLastDigit.FreeRadiusConvertedRoute);
                                }

                                minCodeLastDigit = convertedRouteWithCodeLastDigit.CodeLastDigit;
                            }

                            previousConvertedRouteWithCodeLastDigit = convertedRouteWithCodeLastDigit;
                        }

                        if (string.Compare(minCodeLastDigit, previousConvertedRouteWithCodeLastDigit.CodeLastDigit) != 0)
                        {
                            freeRadiusConvertedRoute.Add(new FreeRadiusConvertedRoute()
                            {
                                Customer_id = customerId,
                                Clisis = "''",
                                Cldsid = string.Format("{0}[{1}-{2}]", codeWithoutLastDigit, minCodeLastDigit, previousConvertedRouteWithCodeLastDigit.CodeLastDigit),
                                Option = freeRadiusConvertedRouteOption.Option,
                                Min_perc = freeRadiusConvertedRouteOption.Min_perc,
                                Max_perc = freeRadiusConvertedRouteOption.Max_perc
                            });
                        }
                        else
                        {
                            freeRadiusConvertedRoute.Add(previousConvertedRouteWithCodeLastDigit.FreeRadiusConvertedRoute);
                        }
                    }
                }
            }

            return freeRadiusConvertedRoute;
        }

        public static List<FreeRadiusConvertedRoute> CompressConvertedRoutes(List<FreeRadiusConvertedRoute> convertedRoutes)
        {
            if (convertedRoutes == null || convertedRoutes.Count == 0)
                return null;

            return CompressConvertedRoutes(BuildConvertedRoutesDict(convertedRoutes));
        }

        private static Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit> BuildConvertedRoutesDict(List<FreeRadiusConvertedRoute> convertedRoutes)
        {
            Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit> results = new Dictionary<string, ConvertedRoutesByCodeWithoutLastDigit>();
            AddToConvertedRoutesDict(results, convertedRoutes);
            return results;
        }

        private static bool AreConsecutives(string firstNumberAsString, string secondNumberAsString)
        {
            int firstNumber = int.Parse(firstNumberAsString);
            int secondNumber = int.Parse(secondNumberAsString);

            if (Math.Abs(firstNumber - secondNumber) == 1)
                return true;

            return false;
        }
    }
}