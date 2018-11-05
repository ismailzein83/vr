using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            var compressedConvertedRoutesByCustomer = new CompressedConvertedRoutesByCustomer();

            foreach (ConvertedRouteWithCode uncompressdRoute in uncompressdRoutes)
            {
                var compressedConvertedRoutesByCodeLength = compressedConvertedRoutesByCustomer.GetOrCreateItem(uncompressdRoute.GetCustomer());
                string currentCode = uncompressdRoute.Code;
                var codeObjectsByParentPrefix = compressedConvertedRoutesByCodeLength.GetOrCreateItem(currentCode.Length);
                string parentCode = currentCode.Substring(0, currentCode.Length - 1);
                Dictionary<string, ConvertedRouteForCompression> routes = codeObjectsByParentPrefix.GetOrCreateItem(parentCode);
                ConvertedRouteForCompression matchedConvertedRouteForCompression = routes.GetOrCreateItem(currentCode, () => { return new ConvertedRouteForCompression() { Code = currentCode, IsUngroupable = IsCodeUngroupable(currentCode, codeGroupManager) }; });
                matchedConvertedRouteForCompression.RouteOptionsIdentifier = uncompressdRoute.GetRouteOptionsIdentifier();

                var prefixes = Enumerable.Range(1, currentCode.Length - 1).Select(p => currentCode.Substring(0, p));

                foreach (string prefix in prefixes)
                {
                    var tempCodeObjectsByParentPrefix = compressedConvertedRoutesByCodeLength.GetOrCreateItem(prefix.Length);
                    string parentPrefixCode = prefix.Substring(0, prefix.Length - 1);
                    Dictionary<string, ConvertedRouteForCompression> prefixRoutes = tempCodeObjectsByParentPrefix.GetOrCreateItem(parentPrefixCode);
                    ConvertedRouteForCompression matchedPrefixConvertedRouteForCompression = prefixRoutes.GetOrCreateItem(prefix, () => { return new ConvertedRouteForCompression() { Code = prefix, IsUngroupable = IsCodeUngroupable(prefix, codeGroupManager) }; });
                }
            }

            foreach (var compressedConvertedRoutesKvp in compressedConvertedRoutesByCustomer)
            {
                string customer = compressedConvertedRoutesKvp.Key;
                var compressedConvertedRoutesByCodeLength = compressedConvertedRoutesKvp.Value;

                List<int> orderedKeys = compressedConvertedRoutesByCodeLength.Keys.OrderByDescending(itm => itm).ToList();
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
                                continue;
                            }

                            var childrenConvertedRoutesForCompression = childrenConvertedRoutesForCompressionByCode.GetRecord(currentCode);
                            if (childrenConvertedRoutesForCompression == null || childrenConvertedRoutesForCompression.Values == null)
                            {
                                continue;
                            }

                            List<ConvertedRouteForCompression> convertedRouteForCompression = childrenConvertedRoutesForCompression.Values.ToList();
                            if (convertedRouteForCompression == null)
                            {
                                continue;
                            }

                            HashSet<string> distinctIdentifiers = Vanrise.Common.ExtensionMethods.ToHashSet(convertedRouteForCompression.Select(itm => itm.RouteOptionsIdentifier));

                            if (distinctIdentifiers.Count > 1)
                            {
                                IEnumerable<ConvertedRouteForCompression> convertedRouteWithSameParentRC = convertedRouteForCompression.Where(itm => itm.RouteOptionsIdentifier == route.RouteOptionsIdentifier && !itm.IsUngroupable);
                                if (convertedRouteWithSameParentRC != null)
                                {
                                    IEnumerable<string> childrenToBeRemoved = convertedRouteWithSameParentRC.Select(itm => itm.Code);
                                    foreach (string codeToRemove in childrenToBeRemoved)
                                        childrenConvertedRoutesForCompression.Remove(codeToRemove);

                                }

                                continue;
                            }

                            string childrenRouteIdentifier = distinctIdentifiers.First();
                            if (string.Compare(route.RouteOptionsIdentifier, childrenRouteIdentifier) != 0 && convertedRouteForCompression.Count != 10)
                                continue;

                            int unGroupableItemsCount = 0;
                            HashSet<string> codesToRemove = new HashSet<string>();

                            foreach (var childConvertedRoutesForCompression in childrenConvertedRoutesForCompression)
                            {
                                if (childConvertedRoutesForCompression.Value.IsUngroupable)
                                    unGroupableItemsCount++;
                                else
                                    codesToRemove.Add(childConvertedRoutesForCompression.Key);
                            }

                            if (unGroupableItemsCount == 0)//All Children are groupable
                            {
                                route.RouteOptionsIdentifier = childrenRouteIdentifier;
                                childrenConvertedRoutesForCompressionByCode.Remove(currentCode);
                            }
                            else
                            {
                                if (childrenConvertedRoutesForCompression.Values.Count == unGroupableItemsCount)//All Children are ungroupable
                                {
                                    continue;
                                }
                                else
                                {
                                    route.RouteOptionsIdentifier = childrenRouteIdentifier;
                                    foreach (string codeToRemove in codesToRemove)
                                        childrenConvertedRoutesForCompression.Remove(codeToRemove);
                                }
                            }
                        }
                    }
                }

                Dictionary<int, Dictionary<string, ConvertedRouteWithCode>> convertedRoutesByCodeLength = new Dictionary<int, Dictionary<string, ConvertedRouteWithCode>>();
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
                                int codeLength = convertedRoute.Code.Length;

                                Dictionary<string, ConvertedRouteWithCode> routes = convertedRoutesByCodeLength.GetOrCreateItem(codeLength, () => { return new Dictionary<string, ConvertedRouteWithCode>(); });
                                routes.Add(convertedRoute.Code, createConvertedRouteWithCode(context));
                            }
                        }
                    }
                }
                IOrderedEnumerable<int> orderedCodeLength = convertedRoutesByCodeLength.Keys.OrderByDescending(itm => itm);
                ConvertedRouteWithCode tempConvertedRouteWithCode;

                List<ConvertedRouteWithCode> finalConvertedRoutes = new List<ConvertedRouteWithCode>();

                foreach (int codeLengthValue in orderedCodeLength)
                {
                    Dictionary<string, ConvertedRouteWithCode> tempRoutes = convertedRoutesByCodeLength.GetRecord(codeLengthValue);

                    foreach (var convertedRouteWithCodeKvp in tempRoutes)
                    {
                        var currentTempRoute = convertedRouteWithCodeKvp.Value;
                        var currentCode = convertedRouteWithCodeKvp.Key;
                        var currentCodeGroup = codeGroupManager.GetMatchCodeGroup(currentCode);

                        if (currentCode == null || currentCode.Equals(currentCodeGroup.Code))
                        {
                            finalConvertedRoutes.Add(currentTempRoute);
                            continue;
                        }

                        var currentCodeGroupLength = currentCodeGroup.Code.Length;

                        var currentRouteOptionIdentifier = currentTempRoute.GetRouteOptionsIdentifier();

                        int previousCodeLengthValue = codeLengthValue - 1;
                        while (previousCodeLengthValue >= currentCodeGroupLength)
                        {
                            string modifiedCode = currentCode.Substring(0, previousCodeLengthValue);

                            Dictionary<string, ConvertedRouteWithCode> parentTempRoutes = convertedRoutesByCodeLength.GetRecord(previousCodeLengthValue);
                            if (parentTempRoutes != null && parentTempRoutes.Count > 0 && parentTempRoutes.TryGetValue(modifiedCode, out tempConvertedRouteWithCode))
                            {
                                if (!currentRouteOptionIdentifier.Equals(tempConvertedRouteWithCode.GetRouteOptionsIdentifier()))
                                    finalConvertedRoutes.Add(currentTempRoute);

                                break;
                            }

                            previousCodeLengthValue--;
                        }

                        if (previousCodeLengthValue < currentCodeGroupLength)
                        {
                            finalConvertedRoutes.Add(currentTempRoute);
                        }
                    }
                }
                if (finalConvertedRoutes.Count > 0)
                    result.AddRange(finalConvertedRoutes);
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
            public bool IsUngroupable { get; set; }
            //public bool CanBeGrouped { get; set; }
        }

        private static bool IsCodeUngroupable(string code, CodeGroupManager codeGroupManager)
        {
            CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code);
            if (codeGroup == null)
                return false;

            return code.Equals(codeGroup.Code);
        }
        #endregion
    }
}