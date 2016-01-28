using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using Vanrise.Caching;

namespace Vanrise.Queueing
{
    public class QueueInstanceManager
    {
        public List<StageName> GetStageNames()
        {
            IQueueInstanceDataManager manager = QDataManagerFactory.GetDataManager<IQueueInstanceDataManager>();
             return manager.GetStageNames();
        }


        public Vanrise.Entities.IDataRetrievalResult<QueueInstanceDetail> GetFilteredQueueInstances(Vanrise.Entities.DataRetrievalInput<QueueInstanceQuery> input)
        {
            var queueExecutionFlows= GetCachedQueueInstances();

            Func<QueueInstance, bool> filterExpression = (queueInstance) =>

                      (input.Query.ExecutionFlowId == null || input.Query.ExecutionFlowId == queueInstance.ExecutionFlowId);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueExecutionFlows.ToBigResult(input, filterExpression, QueueInstanceMapper));

        }

        private Dictionary<int, QueueInstance> GetCachedQueueInstances()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetQueueExecutionFlowDefinitions",
               () =>
               {
                   IQueueInstanceDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueInstanceDataManager>();
                   IEnumerable<QueueInstance> queueExecutionFlowDefinitions = dataManager.GetAll();
                   return queueExecutionFlowDefinitions.ToDictionary(kvp => kvp.QueueInstanceId, kvp => kvp);
               });
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueInstanceDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueInstanceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return true;
                //return _dataManager.AreQueueExecutionFlowDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion


        #region Mappers

        private QueueInstanceDetail QueueInstanceMapper(QueueInstance queueInstance)
        {
            QueueInstanceDetail queueInstanceDetail = new QueueInstanceDetail();
            //QueueExecutionFlowDefinitionManager executionFlowDefinitionManager = new QueueExecutionFlowDefinitionManager();
            queueInstanceDetail.Entity = queueInstance;
            //executionFlowDetail.Title = executionFlowDefinitionManager.GetExecutionFlowDefinitionTitle(executionFlow.DefinitionId);
            return queueInstanceDetail;
        }

        #endregion

    }

     
}
