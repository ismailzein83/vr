using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Common.Business
{
    internal class DataGroupingDistributor
    {
        Dictionary<string, DataAnalysisItem> _dataAnalysisItems = new Dictionary<string, DataAnalysisItem>();

        internal void InitializeDataGroupingAnalysisIfNeeded(string dataAnalysisUniqueName, string serializedGroupingHandler)
        {
            lock (_dataAnalysisItems)
            {
                if (!_dataAnalysisItems.ContainsKey(dataAnalysisUniqueName))
                {
                    DataAnalysisItem dataAnalysisItem = new DataAnalysisItem
                    {
                        DataAnalysisUniqueName = dataAnalysisUniqueName,
                        ExecutorsInfo = new List<DataAnalysisExecutorInfo>()
                    };
                    _dataAnalysisItems.Add(dataAnalysisUniqueName, dataAnalysisItem);
                    Parallel.ForEach(GetExecutorServiceInstances(), (executorServiceInstance) =>
                        {
                            DataGroupingExecutorServiceInstanceInfo executorServiceInstanceInfo = executorServiceInstance.InstanceInfo as DataGroupingExecutorServiceInstanceInfo;
                            if (executorServiceInstanceInfo == null)
                                throw new NullReferenceException("executorServiceInstanceInfo");
                            ServiceClientFactory.CreateTCPServiceClient<IDataGroupingExecutorWCFService>(executorServiceInstanceInfo.TCPServiceURL, (executorClient) =>
                                {
                                    executorClient.InitializeDataGroupingAnalysis(dataAnalysisUniqueName, serializedGroupingHandler);
                                });
                            lock (dataAnalysisItem.ExecutorsInfo)
                            {
                                dataAnalysisItem.ExecutorsInfo.Add(new DataAnalysisExecutorInfo
                                    {
                                        ExecutorServiceInstanceId = executorServiceInstance.ServiceInstanceId
                                    });
                            }
                        });
                }
            }
        }

        internal List<DataGroupingDistributionInfo> GetItemKeysDistributionInfos(string dataAnalysisUniqueName, List<string> itemKeys)
        {
            DataAnalysisItem dataAnalysisItem;
            if (!_dataAnalysisItems.TryGetValue(dataAnalysisUniqueName, out dataAnalysisItem))
                throw new NullReferenceException(String.Format("dataAnalysisItem '{0}'", dataAnalysisUniqueName));

            Dictionary<Guid, DataGroupingDistributionInfo> dataGroupingDistributionsByExecutorServiceId = new Dictionary<Guid, DataGroupingDistributionInfo>();
            if (itemKeys == null)
                Console.WriteLine("itemKeys is null");
            foreach (var itemKey in itemKeys.Distinct())
            {
                Guid? executorServiceInstanceId = null;
                int minItems = int.MaxValue;
                DataAnalysisExecutorInfo executorWithMinItems = null;
                lock (dataAnalysisItem.ExecutorsInfo)
                {
                    foreach (var executorInfo in dataAnalysisItem.ExecutorsInfo)
                    {
                        if (executorInfo.ContainsItemKey(itemKey))
                        {
                            executorServiceInstanceId = executorInfo.ExecutorServiceInstanceId;
                            break;
                        }
                        int itemsCount = executorInfo.ItemsCount;
                        if (itemsCount < minItems)
                        {
                            minItems = itemsCount;
                            executorWithMinItems = executorInfo;
                        }
                    }
                    if (!executorServiceInstanceId.HasValue)
                    {
                        executorWithMinItems.AddItemKey(itemKey);
                        executorServiceInstanceId = executorWithMinItems.ExecutorServiceInstanceId;
                    }
                }
                dataGroupingDistributionsByExecutorServiceId.GetOrCreateItem(executorServiceInstanceId.Value,
                    () => new DataGroupingDistributionInfo { ExecutorServiceInstanceId = executorServiceInstanceId.Value, ItemKeys = new List<string>() })
                    .ItemKeys.Add(itemKey);
            }
            return dataGroupingDistributionsByExecutorServiceId.Values.ToList();
        }

        internal List<Guid> GetExecutorServiceInstanceIds(string dataAnalysisUniqueName)
        {
            DataAnalysisItem dataAnalysisItem;
            if (!_dataAnalysisItems.TryGetValue(dataAnalysisUniqueName, out dataAnalysisItem))
                throw new NullReferenceException(String.Format("dataAnalysisItem '{0}'", dataAnalysisUniqueName));
            return dataAnalysisItem.ExecutorsInfo.Where(itm => itm.ItemsCount > 0).Select(itm => itm.ExecutorServiceInstanceId).ToList();
        }

        private List<ServiceInstance> GetExecutorServiceInstances()
        {
            var executorServices = new ServiceInstanceManager().GetServices(DataGroupingExecutorRuntimeService.s_dataGroupingExecutorServiceInstanceType);
            if (executorServices == null || executorServices.Count == 0)
                throw new NullReferenceException("executorServices");
            return executorServices;
        }

        #region Private Classes

        private class DataAnalysisItem
        {
            public string DataAnalysisUniqueName { get; set; }

            public List<DataAnalysisExecutorInfo> ExecutorsInfo { get; set; }            
        }

        private class DataAnalysisExecutorInfo
        {
            public Guid ExecutorServiceInstanceId { get; set; }

            HashSet<string> _itemsKeys = new HashSet<string>();
            int _itemsCount;
            internal void AddItemKey(string itemKey)
            {
                _itemsKeys.Add(itemKey);
                _itemsCount++;
            }

            internal int ItemsCount
            {
                get
                {
                    return _itemsCount;
                }
            }

            internal bool ContainsItemKey(string itemKey)
            {
                return _itemsKeys.Contains(itemKey);
            }
        }


        #endregion
    }
}
