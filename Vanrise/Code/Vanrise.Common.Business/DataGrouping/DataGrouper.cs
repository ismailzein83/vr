using System;
using System.Collections.Generic;
using System.Configuration;

namespace Vanrise.Common.Business
{
    public abstract class DataGrouper
    {
        protected static int s_distributeBatchSize;
        protected string _dataAnalysisUniqueName;
        protected DataGroupingHandler _groupingHandler;

        static DataGrouper()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["DistributedDataGrouper_DistributeBatchSize"], out s_distributeBatchSize))
                s_distributeBatchSize = 10000;
        }

        public DataGrouper(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler)
        {
            if (dataAnalysisUniqueName == null)
                throw new ArgumentNullException("dataAnalysisUniqueName");
            if (groupingHandler == null)
                throw new ArgumentNullException("groupingHandler");
            _dataAnalysisUniqueName = dataAnalysisUniqueName;
            _groupingHandler = groupingHandler;
        }

        public abstract void DistributeGroupingItems(List<IDataGroupingItem> items);
        public abstract void StartGettingFinalResults(Action<List<dynamic>> onFinalGroupingItemReceived);
        public abstract List<DataGroupingResultDistributionInfo> GetResultDistributionInfo();
        public abstract void StartGettingResultsFromOneExecutor(object executorId, Action<List<dynamic>> onFinalGroupingItemReceived);
    }
}