using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Queueing
{
    public class HoldRequestManager
    {
        public Dictionary<Guid, IOrderedEnumerable<HoldRequest>> GetCachedOrderedHoldRequestsByExecutionFlowDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedOrderedHoldRequestsByExecutionFlowDefinition",
              () =>
              {
                  Dictionary<Guid, List<HoldRequest>> orderedHoldRequestsByExecutionFlowDefinitions = new Dictionary<Guid, List<HoldRequest>>();
                  IOrderedEnumerable<HoldRequest> holdRequests = GetCachedOrderedHoldRequests();
                  foreach (HoldRequest holdRequest in holdRequests)
                  {
                      List<HoldRequest> tempHoldRequests = orderedHoldRequestsByExecutionFlowDefinitions.GetOrCreateItem(holdRequest.ExecutionFlowDefinitionId);
                      tempHoldRequests.Add(holdRequest);
                  }
                  return orderedHoldRequestsByExecutionFlowDefinitions.ToDictionary(itm => itm.Key, itm => itm.Value.OrderBy(x => x.HoldRequestId));
              });

        }

        public IOrderedEnumerable<HoldRequest> GetCachedOrderedHoldRequests()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedOrderedHoldRequests",
              () =>
              {
                  var holdRequests = GetCachedHoldRequests();
                  return holdRequests.Values.OrderBy(itm => itm.HoldRequestId);
              });
        }

        public IEnumerable<HoldRequest> GetAllHoldRequestsByStatus(HoldRequestStatus status)
        {
            var holdRequests = GetCachedHoldRequests();
            return holdRequests.Values.FindAllRecords(itm => itm.Status == status);
        }

        public HoldRequest GetHoldRequest(long bpInstanceId)
        {
            var holdRequests = GetCachedHoldRequests();
            return holdRequests.GetRecord(bpInstanceId);
        }

        public void UpdateStatus(long holdRequestId, HoldRequestStatus status)
        {
            IHoldRequestDataManager dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
            dataManager.UpdateStatus(holdRequestId, status);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public void InsertHoldRequest(long bpInstanceId, Guid executionflowDefinitionId, DateTime from, DateTime to, List<string> stageNamesToHold, List<string> stageNamesToProcess, HoldRequestStatus status)
        {
            QueueExecutionFlowManager queueExecutionFlowManager = new QueueExecutionFlowManager();
            List<QueueExecutionFlow> queueExecutionFlows = queueExecutionFlowManager.GetExecutionFlowsByDefinition(executionflowDefinitionId);

            QueueInstanceManager queueInstanceManager = new QueueInstanceManager();
            IEnumerable<QueueInstance> queueInstances = queueInstanceManager.GetQueueExecutionFlows(queueExecutionFlows.Select(itm => itm.ExecutionFlowId).ToList());

            List<int> queuesToHold = null;
            List<int> queuesToProcess = null;

            if (queueInstances != null && queueInstances.Count() > 0)
            {
                if (stageNamesToHold != null)
                {
                    queuesToHold = new List<int>();
                    foreach (string stageToHold in stageNamesToHold)
                    {
                        QueueInstance queueToHold = queueInstances.FindRecord(itm => string.Compare(itm.StageName, stageToHold) == 0);
                        if (queueToHold != null)
                            queuesToHold.Add(queueToHold.QueueInstanceId);
                    }
                }

                if (stageNamesToProcess != null)
                {
                    queuesToProcess = new List<int>();
                    foreach (string stageToProcess in stageNamesToProcess)
                    {
                        QueueInstance queueToProcess = queueInstances.FindRecord(itm => string.Compare(itm.StageName, stageToProcess) == 0);
                        if (queueToProcess != null)
                            queuesToProcess.Add(queueToProcess.QueueInstanceId);
                    }
                }
            }

            IHoldRequestDataManager dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
            dataManager.Insert(bpInstanceId, executionflowDefinitionId, from, to, queuesToHold, queuesToProcess, status);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public void DeleteHoldRequest(long holdRequestId)
        {
            IHoldRequestDataManager dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
            dataManager.Delete(holdRequestId);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public DateTimeRange GetDBDateTimeRange()
        {
            IHoldRequestDataManager dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
            return dataManager.GetDBDateTimeRange();
        }

        #region Private Methods

        private Dictionary<long, HoldRequest> GetCachedHoldRequests()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedHoldRequests",
               () =>
               {
                   IHoldRequestDataManager dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
                   return dataManager.GetAllHoldRequests().ToDictionary(itm => itm.BPInstanceId, itm => itm);
               });
        }


        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IHoldRequestDataManager _dataManager = QDataManagerFactory.GetDataManager<IHoldRequestDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreHoldRequestsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}