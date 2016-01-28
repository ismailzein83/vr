using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowManager
    {
        public QueuesByStages GetQueuesByStages(int executionFlowId)
        {
            string cacheName = String.Format("QueueExecutionFlowManager_GetQueuesByStages_{0}", executionFlowId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
                   QueueExecutionFlow executionFlow = dataManager.GetExecutionFlow(executionFlowId);
                   List<QueueStageInfo> queueStages = executionFlow.Tree.GetQueueStageInfos();
                   if (queueStages == null || queueStages.Count == 0)
                       throw new Exception("QueueExecutionFlow doesnt return any QueueStageInfo");

                   QueuesByStages queuesByStages = new QueuesByStages();
                   foreach (var stage in queueStages)
                   {
                       var queueName = GetQueueName(executionFlow, stage);
                       var queueTitle = GetQueueTitle(executionFlow, stage);
                       List<string> sourceQueueNames = null;
                       if (stage.SourceQueueStages != null && stage.SourceQueueStages.Count > 0)
                       {
                           sourceQueueNames = new List<string>();
                           foreach (var sourceStage in stage.SourceQueueStages)
                           {
                               sourceQueueNames.Add(GetQueueName(executionFlow, sourceStage));
                           }
                       }
                       PersistentQueueFactory.Default.CreateQueueIfNotExists(executionFlowId, stage.StageName, stage.QueueTypeFQTN, queueName, queueTitle, sourceQueueNames, stage.QueueSettings);
                       if (queuesByStages.ContainsKey(stage.StageName))
                           throw new Exception(String.Format("Duplicate Stage Names: {0}", stage.StageName));
                       queuesByStages.Add(stage.StageName, PersistentQueueFactory.Default.GetQueue(queueName));
                   }
                   return queuesByStages;
               });
            
        }

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDetail> AddExecutionFlow(QueueExecutionFlow executionFlowObj)
        {
            InsertOperationOutput<QueueExecutionFlowDetail> insertOperationOutput = new InsertOperationOutput<QueueExecutionFlowDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int executionFlowId = -1;

            IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
            bool insertActionSucc = dataManager.AddExecutionFlow(executionFlowObj, out executionFlowId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                executionFlowObj.ExecutionFlowId = executionFlowId;
                insertOperationOutput.InsertedObject = QueueExecutionFlowMapper(executionFlowObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        private string GetQueueName(QueueExecutionFlow executionFlow, QueueStageInfo queueStage)
        {
            return String.Format("ExecutionFlow_{0}_Queue_{1}", executionFlow.ExecutionFlowId, queueStage.QueueName);
        }

        private string GetQueueTitle(QueueExecutionFlow executionFlow, QueueStageInfo queueStage)
        {
            return String.Format("{1} ({0})", executionFlow.Name, queueStage.QueueTitle);
        }

        public List<QueueExecutionFlow> GetExecutionFlows()
        {
            var cachedFlows = GetCachedQueueExecutionFlows();
            if (cachedFlows != null)
                return cachedFlows.Values.ToList();
            else
                return null;
        }

        Dictionary<int, QueueExecutionFlow> GetCachedQueueExecutionFlows()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("QueueExecutionFlowManager_GetCachedQueueExecutionFlows",
               () =>
               {
                   IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
                   return dataManager.GetExecutionFlows().ToDictionary(kvp => kvp.ExecutionFlowId, kvp => kvp);
               });
        }


        public Vanrise.Entities.IDataRetrievalResult<QueueExecutionFlowDetail> GetFilteredExecutionFlows(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowQuery> input)
        {
            var queueExecutionFlows = GetCachedQueueExecutionFlows();

            Func<QueueExecutionFlow, bool> filterExpression = (priceList) =>

                      (input.Query.DefinitionId == null || input.Query.DefinitionId == priceList.DefinitionId) &&
                      (input.Query.Name == null || priceList.Name.Contains(input.Query.Name));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input,queueExecutionFlows.ToBigResult(input,filterExpression,QueueExecutionFlowMapper));

        }




        public QueueExecutionFlow GetExecutionFlow(int executionFlowId) 
        {
            var executionFlows = GetCachedQueueExecutionFlows();
            return executionFlows.GetRecord(executionFlowId);
        }


        public Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDetail> UpdateExecutionFlow(QueueExecutionFlow executionFlowObject) 
        {
            IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
            bool updateActionSucc = dataManager.UpdateExecutionFlow(executionFlowObject);
            UpdateOperationOutput<QueueExecutionFlowDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().SetCacheExpired("QueueExecutionFlowManager_GetCachedQueueExecutionFlows");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = QueueExecutionFlowMapper(executionFlowObject);
            }
            return updateOperationOutput;
            
        }


        #region Mappers

        private QueueExecutionFlowDetail QueueExecutionFlowMapper(QueueExecutionFlow executionFlow)
        {
            QueueExecutionFlowDetail executionFlowDetail = new QueueExecutionFlowDetail();
            QueueExecutionFlowDefinitionManager executionFlowDefinitionManager = new QueueExecutionFlowDefinitionManager();
            executionFlowDetail.Entity = executionFlow;
            executionFlowDetail.Title = executionFlowDefinitionManager.GetExecutionFlowDefinitionTitle(executionFlow.DefinitionId);
            return executionFlowDetail;
        }

        #endregion


    }
}
