using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueInstanceManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<QueueInstanceDetail> GetFilteredQueueInstances(Vanrise.Entities.DataRetrievalInput<QueueInstanceQuery> input)
        {
            var queueInstances = GetCachedQueueInstances();

            Func<QueueInstance, bool> filterExpression = (queueInstance) =>

                      (input.Query.ExecutionFlowId == null || input.Query.ExecutionFlowId.Contains((Guid)queueInstance.ExecutionFlowId)) &&
                      (input.Query.Name == null || queueInstance.Name.Contains(input.Query.Name)) &&
                      (input.Query.StageName == null || input.Query.StageName.Contains(queueInstance.StageName)) &&
                      (input.Query.Title == null || queueInstance.Title.Contains(input.Query.Title)) &&
                      (input.Query.ItemTypeId == null || input.Query.ItemTypeId.Contains(queueInstance.ItemTypeId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueInstances.ToBigResult(input, filterExpression, QueueInstanceDetailMapper));

        }

        public IEnumerable<QueueInstance> GetAllQueueInstances()
        {
            return GetCachedQueueInstances().Values;
        }

        public IEnumerable<QueueInstanceInfo> GetQueueInstancesInfo(QueueInstanceFilter filter)
        {
            List<QueueInstance> queueInstances = new List<QueueInstance>();
            if (filter != null)
            {
                List<Guid> filterToList = new List<Guid>();
                filterToList.Add(filter.ExecutionFlowId);
                queueInstances = GetQueueExecutionFlows(filterToList).ToList();
                return queueInstances.MapRecords(QueueInstanceInfoMapper, null);

            }
            queueInstances = GetAllQueueInstances().ToList();
            return queueInstances.MapRecords(QueueInstanceInfoMapper, null);

        }

        public QueueInstance GetQueueInstance(string queueName)
        {
            return GetCachedQueueInstancesByName().GetRecord(queueName);
        }

        public QueueInstance GetQueueInstanceById(int instanceId)
        {
            return GetCachedQueueInstances().GetRecord(instanceId);
        }

        public IEnumerable<QueueInstance> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            var readyQueueInstances = GetReadyQueueInstances();
            if (readyQueueInstances == null)
                return null;
            return readyQueueInstances.Where(itm => queueItemTypes == null || queueItemTypes.Contains(itm.ItemTypeId));
        }

        public IEnumerable<QueueInstance> GetReadyQueueInstances()
        {
            var cachedQueues = GetCachedQueueInstances();
            if (cachedQueues != null)
                return cachedQueues.Values.FindAllRecords(itm => itm.Status == QueueInstanceStatus.ReadyToUse);
            else
                return null;
        }

        public IEnumerable<QueueInstance> GetQueueExecutionFlows(List<Guid> executionFlowIds)
        {
            IEnumerable<QueueInstance> queueInstances = GetCachedQueueInstances().Values.ToList();
            return queueInstances.Where(x => x.ExecutionFlowId.HasValue && executionFlowIds.Contains(x.ExecutionFlowId.Value));

        }


        #endregion


        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueDataManager _queueDataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            object _updateHandle;

            public override Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Caching.CacheObjectSize.ExtraSmall;
                }
            }
            protected override bool ShouldSetCacheExpired()
            {
                return _queueDataManager.AreQueuesUpdated(ref _updateHandle);
            }
        }

        Dictionary<string, QueueInstance> GetCachedQueueInstancesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedQueueInstancesByName",
              () =>
              {
                  IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                  IEnumerable<QueueInstance> queueInstances = dataManager.GetAllQueueInstances();
                  return queueInstances.ToDictionary(kvp => kvp.Name, kvp => kvp);
              });
        }

        Dictionary<int, QueueInstance> GetCachedQueueInstances()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedQueueInstances",
               () =>
               {
                   IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                   IEnumerable<QueueInstance> queueInstances = dataManager.GetAllQueueInstances();
                   return queueInstances.ToDictionary(kvp => kvp.QueueInstanceId, kvp => kvp);
               });
        }

        #endregion


        #region Mappers
        private QueueInstanceDetail QueueInstanceDetailMapper(QueueInstance queueInstance)
        {
            QueueInstanceDetail queueInstanceDetail = new QueueInstanceDetail();
            QueueItemTypeManager itemTypeManager = new QueueItemTypeManager();
            queueInstanceDetail.Entity = queueInstance;
            if (queueInstanceDetail.Entity.ExecutionFlowId != null)
            {
                QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
                queueInstanceDetail.ExecutionFlowName = manager.GetExecutionFlowName((Guid)queueInstanceDetail.Entity.ExecutionFlowId);
            }
            else
            {
                queueInstanceDetail.ExecutionFlowName = string.Empty;
            }

            queueInstanceDetail.StatusName = Vanrise.Common.Utilities.GetEnumDescription(queueInstance.Status);
            queueInstanceDetail.ItemTypeName = itemTypeManager.GetItemTypeName(queueInstanceDetail.Entity.ItemTypeId);

            return queueInstanceDetail;
        }

        private QueueInstanceInfo QueueInstanceInfoMapper(QueueInstance queueInstance)
        {
            QueueInstanceInfo queueInstanceInfo = new QueueInstanceInfo();
            queueInstanceInfo.Id = queueInstance.QueueInstanceId;
            queueInstanceInfo.Title = queueInstance.Title;
            return queueInstanceInfo;
        }

        #endregion
    }
}
