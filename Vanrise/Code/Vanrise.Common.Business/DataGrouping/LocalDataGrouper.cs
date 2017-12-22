using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Vanrise.Common.Business
{

    public class LocalDataGrouper : DataGrouper
    {
        static ConcurrentDictionary<string, DataGroupingItemsDetails> groupingItems = new ConcurrentDictionary<string, DataGroupingItemsDetails>();
        public LocalDataGrouper(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler)
            : base(dataAnalysisUniqueName, groupingHandler)
        {
            if (!groupingItems.ContainsKey(dataAnalysisUniqueName))
            {
                DataGroupingItemsDetails dataGroupingItemsDetails = new DataGroupingItemsDetails() { DataGroupingHandler = groupingHandler, DataGroupingItemsByGroupingKey = new Dictionary<string, IDataGroupingItem>() };
                groupingItems.TryAdd(dataAnalysisUniqueName, dataGroupingItemsDetails);
            }
        }
        public override void DistributeGroupingItems(List<IDataGroupingItem> items)
        {
            if (items == null || items.Count == 0)
                return;

            DataGroupingItemsDetails dataGroupingItemsDetails = groupingItems.GetRecord(_dataAnalysisUniqueName);

            foreach (IDataGroupingItem newItem in items)
            {
                string groupingKey = dataGroupingItemsDetails.DataGroupingHandler.GetItemGroupingKey(new DataGroupingHandlerGetItemGroupingKeyContext() { Item = newItem });

                IDataGroupingItem existingItem;
                if (dataGroupingItemsDetails.DataGroupingItemsByGroupingKey.TryGetValue(groupingKey, out existingItem))
                    dataGroupingItemsDetails.DataGroupingHandler.UpdateExistingItemFromNew(new DataGroupingHandlerUpdateExistingFromNewContext { Existing = existingItem, New = newItem });
                else
                    dataGroupingItemsDetails.DataGroupingItemsByGroupingKey.Add(groupingKey, newItem);
            }
        }

        public override void StartGettingFinalResults(Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            DataGroupingItemsDetails dataGroupingItemsDetails;
            groupingItems.TryRemove(_dataAnalysisUniqueName, out dataGroupingItemsDetails);
            if (dataGroupingItemsDetails != null)
            {
                IEnumerable<IDataGroupingItem> groupedItems = dataGroupingItemsDetails.DataGroupingItemsByGroupingKey.Values;
                List<dynamic> finalResults = groupedItems.Select(itm => itm as dynamic).ToList();
                _groupingHandler.FinalizeGrouping(new DataGroupingHandlerFinalizeGroupingContext() { FinalResults = finalResults, GroupedItems = groupedItems.ToList() });
            }
        }

        public override List<DataGroupingResultDistributionInfo> GetResultDistributionInfo()
        {
            DataGroupingResultDistributionInfo dataGroupingResultDistributionInfo = new DataGroupingResultDistributionInfo() { ExecutorId = Guid.NewGuid() };
            return new List<DataGroupingResultDistributionInfo>() { dataGroupingResultDistributionInfo };
        }

        public override void StartGettingResultsFromOneExecutor(object executorId, Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            StartGettingFinalResults(onFinalGroupingItemReceived);
        }

        private class DataGroupingItemsDetails
        {
            public Dictionary<string, IDataGroupingItem> DataGroupingItemsByGroupingKey { get; set; }
            public DataGroupingHandler DataGroupingHandler { get; set; }
        }
    }
}
