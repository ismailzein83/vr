using System;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class DistributedDataGrouper
    {
        #region ctor

        DataGrouper dataGrouper;

        public DistributedDataGrouper(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler, bool useRemoteDataGrouper)
        {
            if (useRemoteDataGrouper)
                dataGrouper = new RemoteDataGrouper(dataAnalysisUniqueName, groupingHandler);
            else
                dataGrouper = new LocalDataGrouper(dataAnalysisUniqueName, groupingHandler);
        }

        #endregion

        #region Public Methods

        public void DistributeGroupingItems(List<IDataGroupingItem> items)
        {
            dataGrouper.DistributeGroupingItems(items);
        }

        public void StartGettingFinalResults(Action<List<dynamic>> onFinalGroupingItemReceived, Action<LogEntryType, string> logMessage)
        {
            dataGrouper.StartGettingFinalResults(onFinalGroupingItemReceived, logMessage);
        }

        public List<DataGroupingResultDistributionInfo> GetResultDistributionInfo()
        {
            return dataGrouper.GetResultDistributionInfo();
        }

        public void StartGettingResultsFromOneExecutor(object executorId, Action<List<dynamic>> onFinalGroupingItemReceived, Action<LogEntryType, string> logMessage)
        {
            dataGrouper.StartGettingResultsFromOneExecutor(executorId, onFinalGroupingItemReceived, logMessage);
        }

        #endregion
    }

    public class DataGroupingResultDistributionInfo
    {
        public Object ExecutorId { get; set; }
    }
}
