using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    internal class DataGroupingExecutor
    {
        #region Singleton

        static DataGroupingExecutor _current = new DataGroupingExecutor();

        internal static DataGroupingExecutor Current
        {
            get
            {
                return _current;
            }
        }

        private DataGroupingExecutor()
        {

        }

        #endregion

        Dictionary<string, DataAnalysisItem> _dataAnalysisItems = new Dictionary<string, DataAnalysisItem>();
        internal void InitializeDataGroupingAnalysis(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler)
        {
            lock (_dataAnalysisItems)
            {
                if (!_dataAnalysisItems.ContainsKey(dataAnalysisUniqueName))
                {
                    var dataAnalysisItem = new DataAnalysisItem();
                    dataAnalysisItem.DataAnalysisUniqueName = dataAnalysisUniqueName;
                    dataAnalysisItem.GroupingHandler = groupingHandler;
                    dataAnalysisItem.ItemsByKey = new Dictionary<string, IDataGroupingItem>();
                    _dataAnalysisItems.Add(dataAnalysisUniqueName, dataAnalysisItem);
                }
            }
        }

        internal void AddItemsToGrouping(string dataAnalysisUniqueName, string serializedItems)
        {
            DataAnalysisItem dataAnalysisItem;
            if (!_dataAnalysisItems.TryGetValue(dataAnalysisUniqueName, out dataAnalysisItem))
                throw new Exception(String.Format("dataAnalysisItem '{0}' is not found", dataAnalysisUniqueName));
           
            dataAnalysisItem.AddItemsToQueue(serializedItems);
        }

        internal DataGroupingExecutorFinalResult GetFinalResults(string dataAnalysisUniqueName, int nbOfItems)
        {
            DataAnalysisItem dataAnalysisItem;
            if (!_dataAnalysisItems.TryGetValue(dataAnalysisUniqueName, out dataAnalysisItem))
                throw new Exception(String.Format("dataAnalysisItem '{0}' is not found", dataAnalysisUniqueName));
            DataGroupingExecutorFinalResult rslt = new DataGroupingExecutorFinalResult();
            List<dynamic> itemsToReturn = null;

            lock(dataAnalysisItem)
            {
                if (dataAnalysisItem.FinalResult == null && dataAnalysisItem.ItemsByKey != null)
                {
                    while(dataAnalysisItem.Queue.Count > 0 || dataAnalysisItem.IsRunning)
                    {
                        System.Threading.Thread.Sleep(250);
                    }
                    DataGroupingHandlerFinalizeGroupingContext finalizeGroupingContext = new DataGroupingHandlerFinalizeGroupingContext
                    {
                        GroupedItems = dataAnalysisItem.ItemsByKey.Values.ToList()
                    };
                    dataAnalysisItem.GroupingHandler.FinalizeGrouping(finalizeGroupingContext);
                    dataAnalysisItem.FinalResult = finalizeGroupingContext.FinalResults;
                    dataAnalysisItem.ItemsByKey = null;
                }
                if (dataAnalysisItem.FinalResult != null)
                {
                    itemsToReturn = dataAnalysisItem.FinalResult.Take(nbOfItems).ToList();
                    dataAnalysisItem.FinalResult.RemoveRange(0, itemsToReturn.Count);
                    rslt.HasMoreResult = dataAnalysisItem.FinalResult.Count > 0;
                }
                if(!rslt.HasMoreResult)
                    _dataAnalysisItems.Remove(dataAnalysisUniqueName);
            }
            if (itemsToReturn != null)
                rslt.SerializedItems = dataAnalysisItem.GroupingHandler.SerializeFinalResultItems(itemsToReturn);
            return rslt;
        }

        internal void ProcessItems()
        {
            foreach(var dataAnalysisItem in _dataAnalysisItems.Values)
            {
                dataAnalysisItem.ScheduleProcessingIfNeeded();
            }
        }

        #region Private Classes

        private class DataAnalysisItem
        {
            Object _lockObj = new object();

            public bool IsRunning
            {
                get
                {
                    return _isRunning;
                }
            }
            bool _isRunning;
           
            internal void ScheduleProcessingIfNeeded()
            {
                if(!_isRunning && _queueItems.Count > 0)
                {
                    Task t = new Task(() =>
                    {
                        lock (_lockObj)
                        {
                            if (_isRunning)
                                return;
                            _isRunning = true;
                        }
                        string serializedItems;
                        while (_queueItems.TryDequeue(out serializedItems))
                        {
                            var items = GroupingHandler.DeserializeItems(serializedItems);
                            if (items == null)
                                throw new NullReferenceException("items");
                            Console.WriteLine("{0} items '{1}'", items.Count, DataAnalysisUniqueName);
                            foreach (var item in items)
                            {
                                IDataGroupingItem matchItem;
                                string groupingKey = GroupingHandler.GetItemGroupingKey(new DataGroupingHandlerGetItemGroupingKeyContext { Item = item });
                                if (!ItemsByKey.TryGetValue(groupingKey, out matchItem))
                                {
                                    ItemsByKey.Add(groupingKey, item);
                                }
                                else
                                {
                                    GroupingHandler.UpdateExistingItemFromNew(new DataGroupingHandlerUpdateExistingFromNewContext
                                    {
                                        Existing = matchItem,
                                        New = item
                                    });
                                }
                            }
                            Console.WriteLine("{0} Total Grouped items '{1}'", ItemsByKey.Count, DataAnalysisUniqueName);
                        }        
                        lock (_lockObj)
                        {
                            _isRunning = false;
                        }
                    });
                    t.Start();
                }
            }

            public string DataAnalysisUniqueName { get; set; }

            public DataGroupingHandler GroupingHandler { get; set; }

            public Dictionary<string, IDataGroupingItem> ItemsByKey { get; set; }

            public List<dynamic> FinalResult { get; set; }

            ConcurrentQueue<string> _queueItems = new ConcurrentQueue<string>();

            internal ConcurrentQueue<string> Queue
            {
                get
                {
                    return _queueItems;
                }
            }

            internal void AddItemsToQueue(string serializedItems)
            {
                _queueItems.Enqueue(serializedItems);
            }
        }
        #endregion
    }
}
