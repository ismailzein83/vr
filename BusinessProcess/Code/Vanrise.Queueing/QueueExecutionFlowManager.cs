﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
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

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlow> AddExecutionFlow(QueueExecutionFlow executionFlowObj)
        {
            InsertOperationOutput<QueueExecutionFlow> insertOperationOutput = new InsertOperationOutput<QueueExecutionFlow>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int executionFlowId = -1;

            IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
            bool insertActionSucc = dataManager.AddExecutionFlow(executionFlowObj, out executionFlowId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                executionFlowObj.ExecutionFlowId = executionFlowId;
                insertOperationOutput.InsertedObject = executionFlowObj;
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
    }
}
