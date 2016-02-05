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
        public Vanrise.Entities.IDataRetrievalResult<QueueInstanceDetail> GetFilteredQueueInstances(Vanrise.Entities.DataRetrievalInput<QueueInstanceQuery> input)
        {
            var queueInstances = GetCachedQueueInstances();

            Func<QueueInstance, bool> filterExpression = (queueInstance) =>

                      (input.Query.ExecutionFlowId == null || input.Query.ExecutionFlowId.Contains((int)queueInstance.ExecutionFlowId)) &&
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

        Dictionary<int, QueueInstance> GetCachedQueueInstances()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("GetCachedQueueInstances",
               () =>
               {
                   IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                   IEnumerable<QueueInstance> queueInstances = dataManager.GetAllQueueInstances();
                   return queueInstances.ToDictionary(kvp => kvp.QueueInstanceId, kvp => kvp);
               });
        }

        public IEnumerable<QueueInstanceInfo> GetQueueInstances(QueueInstanceFilter filter)
        {
            IEnumerable<QueueInstance> queueInstances = GetAllQueueInstances();
            if (filter != null)
                return queueInstances.MapRecords(QueueInstanceInfoMapper, queueInstance => queueInstance.ExecutionFlowId == filter.ExecutionFlowId);

            return queueInstances.MapRecords(QueueInstanceInfoMapper, null);

        }

        #region Mappers
        private QueueInstanceDetail QueueInstanceDetailMapper(QueueInstance queueInstance)
        {
            QueueInstanceDetail queueInstanceDetail = new QueueInstanceDetail();
            QueueItemTypeManager itemTypeManager = new QueueItemTypeManager();
            queueInstanceDetail.Entity = queueInstance;
            if (queueInstanceDetail.Entity.ExecutionFlowId != null)
            {
                QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
                queueInstanceDetail.ExecutionFlowName = manager.GetExecutionFlowName((int)queueInstanceDetail.Entity.ExecutionFlowId);
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
            queueInstanceInfo.Name = queueInstance.Name;
            return queueInstanceInfo;
        }

        #endregion
    }
}
