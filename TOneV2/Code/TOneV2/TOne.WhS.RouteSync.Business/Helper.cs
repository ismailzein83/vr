using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Business
{
    public static class Helper
    {
        public static SwitchSyncOutput MergeSwitchSyncOutputItems(List<SwitchSyncOutput> switchSyncOutputList)
        {
            if (switchSyncOutputList == null || switchSyncOutputList.Count == 0)
                return null;

            HashSet<string> distinctSwitchIds = Vanrise.Common.ExtensionMethods.ToHashSet(switchSyncOutputList.Select(itm => itm.SwitchId));
            if (distinctSwitchIds.Count > 1)
                throw new Exception("switchSyncOutputList items should be related to one Switch.");

            SwitchSyncOutput result = new SwitchSyncOutput() { SwitchId = distinctSwitchIds.First(), SwitchSyncResult = SwitchSyncResult.Succeed, SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>() };
            foreach (SwitchSyncOutput switchSyncOutput in switchSyncOutputList)
            {
                result.SwitchSyncResult = (int)result.SwitchSyncResult > (int)switchSyncOutput.SwitchSyncResult ? result.SwitchSyncResult : switchSyncOutput.SwitchSyncResult;
                if (switchSyncOutput.SwitchRouteSynchroniserOutputList != null)
                    result.SwitchRouteSynchroniserOutputList.AddRange(switchSyncOutput.SwitchRouteSynchroniserOutputList);
            }
            return result;
        }

        public static SwitchSyncOutput MergeSwitchSyncOutputItems(SwitchSyncOutput firstItem, SwitchSyncOutput secondItem)
        {
            List<SwitchSyncOutput> items = new List<SwitchSyncOutput>();
            if (firstItem != null)
                items.Add(firstItem);

            if (secondItem != null)
                items.Add(secondItem);

            return MergeSwitchSyncOutputItems(items);
        }

        #region Compression
        public static List<ConvertedRoute> CompressRoutesWithCodes(IEnumerable<ConvertedRouteWithCode> uncompressdRoutes, Func<ICreateConvertedRouteWithCodeContext, ConvertedRouteWithCode> createConvertedRouteWithCode)
        {
            List<ConvertedRoute> result = new List<ConvertedRoute>();

            var compressedConvertedRoutesByCustomer = new CompressedConvertedRoutesByCustomer();

            foreach (ConvertedRouteWithCode uncompressdRoute in uncompressdRoutes)
            {
                var compressedConvertedRoutesByCodeLength = compressedConvertedRoutesByCustomer.GetOrCreateItem(uncompressdRoute.GetCustomer());
                string currentCode = uncompressdRoute.Code;
                var codeObjectsByParentPrefix = compressedConvertedRoutesByCodeLength.GetOrCreateItem(currentCode.Length);
                string parentCode = currentCode.Substring(0, currentCode.Length - 1);
                Dictionary<string, ConvertedRouteForCompression> routes = codeObjectsByParentPrefix.GetOrCreateItem(parentCode);
                ConvertedRouteForCompression matchedConvertedRouteForCompression = routes.GetOrCreateItem(currentCode, () => { return new ConvertedRouteForCompression() { Code = currentCode }; });
                matchedConvertedRouteForCompression.RouteOptionsIdentifier = uncompressdRoute.GetRouteOptionsIdentifier();

                var prefixes = Enumerable.Range(1, currentCode.Length - 1).Select(p => currentCode.Substring(0, p));

                foreach (string prefix in prefixes)
                {
                    var tempCodeObjectsByParentPrefix = compressedConvertedRoutesByCodeLength.GetOrCreateItem(prefix.Length);
                    string parentPrefixCode = prefix.Substring(0, prefix.Length - 1);
                    Dictionary<string, ConvertedRouteForCompression> prefixRoutes = tempCodeObjectsByParentPrefix.GetOrCreateItem(parentPrefixCode);
                    ConvertedRouteForCompression matchedPrefixConvertedRouteForCompression = prefixRoutes.GetOrCreateItem(prefix, () => { return new ConvertedRouteForCompression() { Code = prefix }; });
                }
            }

            foreach (var compressedConvertedRoutesKvp in compressedConvertedRoutesByCustomer)
            {
                string customer = compressedConvertedRoutesKvp.Key;
                var compressedConvertedRoutesByCodeLength = compressedConvertedRoutesKvp.Value;

                List<int> orderedKeys = compressedConvertedRoutesByCodeLength.Keys.OrderByDescending(itm => itm).ToList();
                Dictionary<string, Dictionary<string, ConvertedRouteForCompression>> longestConvertedRoutes = compressedConvertedRoutesByCodeLength.GetRecord(orderedKeys[0]);
                foreach (var longestConvertedRoutesKvp in longestConvertedRoutes)
                {
                    Dictionary<string, ConvertedRouteForCompression> routes = longestConvertedRoutesKvp.Value;
                    foreach (ConvertedRouteForCompression route in routes.Values)
                        route.CanBeGrouped = true;
                }

                for (int i = 1; i < orderedKeys.Count; i++)
                {
                    int currentKey = orderedKeys[i];
                    int childrenKey = currentKey + 1;

                    Dictionary<string, Dictionary<string, ConvertedRouteForCompression>> currentConvertedRoutesForCompressionByCode = compressedConvertedRoutesByCodeLength.GetRecord(currentKey);
                    Dictionary<string, Dictionary<string, ConvertedRouteForCompression>> childrenConvertedRoutesForCompressionByCode = compressedConvertedRoutesByCodeLength.GetRecord(childrenKey);
                    foreach (var convertedRoutesKvp in currentConvertedRoutesForCompressionByCode)
                    {
                        List<ConvertedRouteForCompression> routes = convertedRoutesKvp.Value.Values.ToList();

                        for (int j = routes.Count - 1; j >= 0; j--)
                        {
                            ConvertedRouteForCompression route = routes[j];
                            string currentCode = route.Code;
                            if (childrenConvertedRoutesForCompressionByCode == null)
                            {
                                route.CanBeGrouped = true;
                                continue;
                            }

                            var tempResult = childrenConvertedRoutesForCompressionByCode.GetRecord(currentCode);
                            if (tempResult == null || tempResult.Values == null)
                            {
                                route.CanBeGrouped = true;
                                continue;
                            }

                            List<ConvertedRouteForCompression> convertedRouteForCompression = tempResult.Values.ToList();
                            if (convertedRouteForCompression == null)
                            {
                                route.CanBeGrouped = true;
                                continue;
                            }

                            if (convertedRouteForCompression.Count != 10 || convertedRouteForCompression.FirstOrDefault(itm => !itm.CanBeGrouped) != null)
                            {
                                convertedRoutesKvp.Value.Remove(route.Code);
                                //routes.Remove(route);
                                continue;
                            }

                            HashSet<string> distinctIdentifiers = Vanrise.Common.ExtensionMethods.ToHashSet(convertedRouteForCompression.Select(itm => itm.RouteOptionsIdentifier));
                            if (distinctIdentifiers.Count > 1)
                            {
                                convertedRoutesKvp.Value.Remove(route.Code);
                                //routes.Remove(route);
                                continue;
                            }

                            route.RouteOptionsIdentifier = distinctIdentifiers.First();
                            route.CanBeGrouped = true;
                            childrenConvertedRoutesForCompressionByCode.Remove(currentCode);
                        }
                    }
                }

                foreach (var compressedConvertedRoutesByCodeLengthKvp in compressedConvertedRoutesByCodeLength)
                {
                    var compressedConvertedRoutesByParentCode = compressedConvertedRoutesByCodeLengthKvp.Value;

                    foreach (var compressedConvertedRoutesByParentCodeKvp in compressedConvertedRoutesByParentCode)
                    {
                        var convertedRouteForCompressionDict = compressedConvertedRoutesByParentCodeKvp.Value;
                        foreach (var convertedRouteKvp in convertedRouteForCompressionDict)
                        {
                            var convertedRoute = convertedRouteKvp.Value;
                            if (!string.IsNullOrEmpty(convertedRoute.RouteOptionsIdentifier))
                            {
                                CreateConvertedRouteWithCodeContext context = new CreateConvertedRouteWithCodeContext()
                                {
                                    Code = convertedRoute.Code,
                                    Customer = customer,
                                    RouteOptionIdentifier = convertedRoute.RouteOptionsIdentifier
                                };
                                result.Add(createConvertedRouteWithCode(context));
                            }
                        }
                    }

                }
            }

            return result;
        }

        private class CompressedConvertedRoutesByParentCode : Dictionary<string, Dictionary<string, ConvertedRouteForCompression>>
        {

        }

        private class CompressedConvertedRoutesByCustomer : Dictionary<string, CompressedConvertedRoutes>
        {
        }
        private class CompressedConvertedRoutes : Dictionary<int, CompressedConvertedRoutesByParentCode>
        {
        }

        private class ConvertedRouteForCompression
        {
            public string Code { get; set; }
            public string RouteOptionsIdentifier { get; set; }
            public bool CanBeGrouped { get; set; }
        }
        #endregion
    }
}