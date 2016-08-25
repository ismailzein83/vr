using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    public class DistributedDataGrouper
    {
        #region ctor

        string _dataAnalysisUniqueName;
        DataGroupingHandler _groupingHandler;

        public DistributedDataGrouper(string dataAnalysisUniqueName, DataGroupingHandler groupingHandler)
        {
            if (dataAnalysisUniqueName == null)
                throw new ArgumentNullException("dataAnalysisUniqueName");
            if (groupingHandler == null)
                throw new ArgumentNullException("groupingHandler");
            _dataAnalysisUniqueName = dataAnalysisUniqueName;
            _groupingHandler = groupingHandler;
            GetDistributorClient((distributorClient) =>
            {
                distributorClient.InitializeDataGroupingAnalysisIfNeeded(dataAnalysisUniqueName,  Serializer.Serialize(groupingHandler));
            });
        }

        #endregion

        #region Public Methods

        public void DistributeGroupingItems(List<IDataGroupingItem> items)
        {
            List<DataGroupingDistributionInfo> distributionInfos = null;
            Dictionary<string, IDataGroupingItem> itemsByKeys = items.ToDictionary(itm => _groupingHandler.GetItemGroupingKey(new DataGroupingHandlerGetItemGroupingKeyContext { Item = itm }), itm => itm);
            GetDistributorClient((distributorClient) =>
            {
                distributionInfos = distributorClient.GetItemKeysDistributionInfos(_dataAnalysisUniqueName, itemsByKeys.Keys.ToList());
            });
            Parallel.ForEach(distributionInfos, (distributionInfo) =>
            {
                GetExecutorClient(distributionInfo.ExecutorServiceInstanceId, (executorClient) =>
                {
                    executorClient.AddItemsToGrouping(_dataAnalysisUniqueName, _groupingHandler.SerializeItems(itemsByKeys.Where(itmEntry => distributionInfo.ItemKeys.Contains(itmEntry.Key)).Select(itmEntry => itmEntry.Value).ToList()));
                });
            });
        }

        public void StartGettingFinalResults(Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            List<Guid> executorServiceInstanceIds = GetAnalysisExecutorIds();
            Parallel.ForEach(executorServiceInstanceIds, (executorServiceInstanceId) =>
            {
                StartGettingFinalResultsFromExecutor(executorServiceInstanceId, onFinalGroupingItemReceived);
            });
        }

        public List<DataGroupingResultDistributionInfo> GetResultDistributionInfo()
        {
            return GetAnalysisExecutorIds().Select(executorId => new DataGroupingResultDistributionInfo { ExecutorId = executorId }).ToList();
        }

        public void StartGettingResultsFromOneExecutor(object executorId, Action<List<dynamic>> onFinalGroupingItemReceived)
        {
            if (!(executorId is Guid))
                throw new Exception(String.Format("Invalid executorId '{0}'", executorId));
            StartGettingFinalResultsFromExecutor((Guid)executorId, onFinalGroupingItemReceived);
        }

        #endregion

        #region Private Methods

        private void GetDistributorClient(Action<IDataGroupingDistributorWCFService> onClientReady)
        {
            if (_distributorServiceURL == null)
                AssignDistributor();
            ServiceClientFactory.CreateTCPServiceClient<IDataGroupingDistributorWCFService>(_distributorServiceURL, onClientReady);
        }

        private void GetExecutorClient(Guid executorServiceInstanceId, Action<IDataGroupingExecutorWCFService> onClientReady)
        {
            var executorServiceInstance = new ServiceInstanceManager().GetServiceInstance(DataGroupingExecutorRuntimeService.s_dataGroupingExecutorServiceInstanceType, executorServiceInstanceId);
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
            ServiceInstance distributorServiceInstance = null;
            var serviceInstanceManager = new ServiceInstanceManager();
            if (dataManagerAnalysisInfo.TryGetAssignedServiceInstanceId(_dataAnalysisUniqueName, out distributorServiceInstanceId))
                distributorServiceInstance = serviceInstanceManager.GetServiceInstance(DataGroupingDistributorRuntimeService.s_dataGroupingDistributorServiceInstanceType, distributorServiceInstanceId);
            else
            {
                Dictionary<Guid, ServiceInstance> distributorProcessServiceInstances = serviceInstanceManager.GetServicesDictionary(DataGroupingDistributorRuntimeService.s_dataGroupingDistributorServiceInstanceType);
                if (distributorProcessServiceInstances == null || distributorProcessServiceInstances.Count == 0)
                    throw new NullReferenceException("distributorProcessServiceInstances");
                Dictionary<Guid, int> dataAnalysisCountByServiceInstanceId = dataManagerAnalysisInfo.GetDataAnalysisCountByServiceInstanceId();
                ServiceInstance firstUnassignedDistributor = distributorProcessServiceInstances.Values.FirstOrDefault(itm => !dataAnalysisCountByServiceInstanceId.ContainsKey(itm.ServiceInstanceId));
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

    public class DataGroupingResultDistributionInfo
    {
        public Object ExecutorId { get; set; }
    }
}
