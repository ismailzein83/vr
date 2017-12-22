using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class RemoteDataGrouper : DataGrouper
    {
        Dictionary<string, Guid> _executerServiceIdByGroupingKey = new Dictionary<string, Guid>();

        public RemoteDataGrouper(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler)
            : base(dataAnalysisUniqueName, groupingHandler)
        {
            GetDistributorClient((distributorClient) =>
            {
                distributorClient.InitializeDataGroupingAnalysisIfNeeded(dataAnalysisUniqueName, Serializer.Serialize(groupingHandler));
            });
        }
        public override void DistributeGroupingItems(List<IDataGroupingItem> items)
        {
            int itemsCount = items.Count;
            if (itemsCount <= s_distributeBatchSize)
                DistributeGroupingItems_Private(items);
            else
            {
                for (int i = 0; i < itemsCount; i += s_distributeBatchSize)
                {
                    int start = i;
                    int countToGet = Math.Min(s_distributeBatchSize, itemsCount - i);
                    DistributeGroupingItems_Private(items.GetRange(start, countToGet).ToList());
                }
            }
        }

        public override void StartGettingFinalResults(Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            List<Guid> executorServiceInstanceIds = GetAnalysisExecutorIds();
            Parallel.ForEach(executorServiceInstanceIds, (executorServiceInstanceId) =>
            {
                StartGettingFinalResultsFromExecutor(executorServiceInstanceId, onFinalGroupingItemReceived);
            });
        }

        public override List<DataGroupingResultDistributionInfo> GetResultDistributionInfo()
        {
            return GetAnalysisExecutorIds().Select(executorId => new DataGroupingResultDistributionInfo { ExecutorId = executorId }).ToList();
        }

        public override void StartGettingResultsFromOneExecutor(object executorId, Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            if (!(executorId is Guid))
                throw new Exception(String.Format("Invalid executorId '{0}'", executorId));
            StartGettingFinalResultsFromExecutor((Guid)executorId, onFinalGroupingItemReceived);
        }

        #region Private Methods
        private void DistributeGroupingItems_Private(List<IDataGroupingItem> items)
        {
            Dictionary<string, IDataGroupingItem> itemsByKeys = items.ToDictionary(itm => _groupingHandler.GetItemGroupingKey(new DataGroupingHandlerGetItemGroupingKeyContext { Item = itm }), itm => itm);
            IEnumerable<string> keysToRequest = itemsByKeys.Keys.Where(itm => !_executerServiceIdByGroupingKey.ContainsKey(itm));
            if (keysToRequest != null && keysToRequest.Count() > 0)
            {
                GetDistributorClient((distributorClient) =>
                {
                    var receivedDistributionInfos = distributorClient.GetItemKeysDistributionInfos(_dataAnalysisUniqueName, keysToRequest.ToList());
                    foreach (var distributionInfo in receivedDistributionInfos)
                    {
                        foreach (var groupingKey in distributionInfo.ItemKeys)
                        {
                            _executerServiceIdByGroupingKey.Add(groupingKey, distributionInfo.ExecutorServiceInstanceId);
                        }
                    }
                });
            }

            Dictionary<Guid, List<IDataGroupingItem>> itemsByExecutorServiceId = new Dictionary<Guid, List<IDataGroupingItem>>();
            foreach (var groupingItemEntry in itemsByKeys)
            {
                itemsByExecutorServiceId.GetOrCreateItem(_executerServiceIdByGroupingKey[groupingItemEntry.Key]).Add(groupingItemEntry.Value);
            }

            Parallel.ForEach(itemsByExecutorServiceId, (gourpingItemsEntry) =>
            {
                GetExecutorClient(gourpingItemsEntry.Key, (executorClient) =>
                {
                    executorClient.AddItemsToGrouping(_dataAnalysisUniqueName, _groupingHandler.SerializeItems(gourpingItemsEntry.Value));
                });
            });
        }

        private void GetDistributorClient(Action<IDataGroupingDistributorWCFService> onClientReady)
        {
            if (_distributorServiceURL == null)
                AssignDistributor();
            ServiceClientFactory.CreateTCPServiceClient<IDataGroupingDistributorWCFService>(_distributorServiceURL, onClientReady);
        }

        private void GetExecutorClient(Guid executorServiceInstanceId, Action<IDataGroupingExecutorWCFService> onClientReady)
        {
            var executorServiceInstance = new RuntimeServiceInstanceManager().GetServiceInstance(DataGroupingExecutorRuntimeService.SERVICE_TYPE_UNIQUE_NAME, executorServiceInstanceId);
            if (executorServiceInstance == null)
                throw new NullReferenceException(String.Format("executorServiceInstance '{0}'", executorServiceInstanceId));
            DataGroupingExecutorServiceInstanceInfo executorServiceInstanceInfo = executorServiceInstance.InstanceInfo as DataGroupingExecutorServiceInstanceInfo;
            if (executorServiceInstanceInfo == null)
                throw new NullReferenceException("executorServiceInstanceInfo");
            ServiceClientFactory.CreateTCPServiceClient<IDataGroupingExecutorWCFService>(executorServiceInstanceInfo.TCPServiceURL, onClientReady);
        }

        string _distributorServiceURL;
        void AssignDistributor()
        {
            IDataGroupingAnalysisInfoDataManager dataManagerAnalysisInfo = CommonDataManagerFactory.GetDataManager<IDataGroupingAnalysisInfoDataManager>();
            Guid distributorServiceInstanceId;
            RuntimeServiceInstance distributorServiceInstance = null;
            var serviceInstanceManager = new RuntimeServiceInstanceManager();
            if (dataManagerAnalysisInfo.TryGetAssignedServiceInstanceId(_dataAnalysisUniqueName, out distributorServiceInstanceId))
                distributorServiceInstance = serviceInstanceManager.GetServiceInstance(DataGroupingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME, distributorServiceInstanceId);
            else
            {
                Dictionary<Guid, RuntimeServiceInstance> distributorProcessServiceInstances = serviceInstanceManager.GetServicesDictionary(DataGroupingDistributorRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
                if (distributorProcessServiceInstances == null || distributorProcessServiceInstances.Count == 0)
                    throw new NullReferenceException("distributorProcessServiceInstances");
                Dictionary<Guid, int> dataAnalysisCountByServiceInstanceId = dataManagerAnalysisInfo.GetDataAnalysisCountByServiceInstanceId();
                RuntimeServiceInstance firstUnassignedDistributor = distributorProcessServiceInstances.Values.FirstOrDefault(itm => !dataAnalysisCountByServiceInstanceId.ContainsKey(itm.ServiceInstanceId));
                if (firstUnassignedDistributor != null)
                    distributorServiceInstanceId = firstUnassignedDistributor.ServiceInstanceId;
                else
                    distributorServiceInstanceId = dataAnalysisCountByServiceInstanceId.Where(itm => distributorProcessServiceInstances.ContainsKey(itm.Key)).OrderBy(itm => itm.Value).First().Key;
                dataManagerAnalysisInfo.TryAssignServiceInstanceId(_dataAnalysisUniqueName, ref distributorServiceInstanceId);
                distributorServiceInstance = distributorProcessServiceInstances.GetRecord(distributorServiceInstanceId);
            }
            DataGroupingDistributorServiceInstanceInfo distributorServiceInstanceInfo = distributorServiceInstance.InstanceInfo as DataGroupingDistributorServiceInstanceInfo;
            if (distributorServiceInstanceInfo == null)
                throw new NullReferenceException("distributorServiceInstanceInfo");
            _distributorServiceURL = distributorServiceInstanceInfo.TCPServiceURL;
        }

        private List<Guid> GetAnalysisExecutorIds()
        {
            List<Guid> executorServiceInstanceIds = null;
            GetDistributorClient((distributorClient) =>
            {
                executorServiceInstanceIds = distributorClient.GetExecutorServiceInstanceIds(_dataAnalysisUniqueName);
            });
            return executorServiceInstanceIds;
        }

        private void StartGettingFinalResultsFromExecutor(Guid executorServiceInstanceId, Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            GetExecutorClient(executorServiceInstanceId, (executorClient) =>
            {
                DataGroupingExecutorFinalResult finalResult = null;
                do
                {
                    finalResult = executorClient.GetFinalResults(_dataAnalysisUniqueName, 1000);
                    if (finalResult != null && finalResult.SerializedItems != null)
                    {
                        var items = _groupingHandler.DeserializeFinalResultItems(finalResult.SerializedItems);
                        onFinalGroupingItemReceived(items);
                    }
                }
                while (finalResult != null && finalResult.HasMoreResult);
            });
        }

        #endregion
    }
}