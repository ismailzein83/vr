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

        public QueuesByStages GetQueuesByStages2(int executionFlowId)
        {
            string cacheName = String.Format("QueueExecutionFlowManager_GetQueuesByStages2_{0}", executionFlowId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();                   
                   QueueExecutionFlow executionFlow = dataManager.GetExecutionFlow(executionFlowId);

                   if (executionFlow == null)
                       throw new ArgumentNullException(String.Format("Execution Flow of ID '{0}'", executionFlowId));

                   QueueExecutionFlowDefinitionManager definitionManager = new QueueExecutionFlowDefinitionManager();
                   var executionFlowDefinition = definitionManager.GetExecutionFlowDefinition(executionFlow.DefinitionId);
                   if(executionFlowDefinition == null)
                       throw new ArgumentNullException(String.Format("Execution Flow Definition of ID '{0}'", executionFlow.DefinitionId));

                   if (executionFlowDefinition.Stages == null)
                       throw new ArgumentNullException("executionFlowDefinition.Stages");

                   CheckRecursiveSources(executionFlowDefinition);
                   foreach(var stage in executionFlowDefinition.Stages)
                   {
                       CreateStageQueueIfNeeded(stage, executionFlow, executionFlowDefinition);
                   }

                   QueuesByStages queuesByStages = new QueuesByStages();
                   foreach (var stage in executionFlowDefinition.Stages)
                   {
                       string queueName = BuildQueueName(stage, executionFlow);
                       if (queuesByStages.ContainsKey(stage.StageName))
                           throw new Exception(String.Format("Duplicate Stage Names: {0}", stage.StageName));
                       queuesByStages.Add(stage.StageName, PersistentQueueFactory.Default.GetQueue(queueName));
                   }
                   return queuesByStages;
               });

        }

        private void CheckRecursiveSources(QueueExecutionFlowDefinition executionFlowDefinition)
        {
            foreach (var stage in executionFlowDefinition.Stages)
            {
                if (stage.SourceStages != null)
                {
                    foreach (var sourceStageName in stage.SourceStages)
                    {
                        List<string> prohibitedSources = new List<string>();
                        prohibitedSources.Add(stage.StageName);
                        CheckRecursiveSources(executionFlowDefinition, sourceStageName, prohibitedSources);
                    }
                }
            }
        }

        private void CheckRecursiveSources(QueueExecutionFlowDefinition executionFlowDefinition, string sourceStageName, List<string> prohibitedSources)
        {
            if (prohibitedSources.Contains(sourceStageName))
                throw new Exception(String.Format("Execution Flow '{0}' has recursive source stages", executionFlowDefinition.Name));
            var sourceStage = executionFlowDefinition.Stages.FindRecord(itm => itm.StageName == sourceStageName);
            if (sourceStage == null)
                throw new Exception(String.Format("Stage '{0}' not found in Execution Flow '{1}'", sourceStageName, executionFlowDefinition.Name));
            prohibitedSources.Add(sourceStage.StageName);
            if(sourceStage.SourceStages != null)
                foreach (var sourceOfSourceStageName in sourceStage.SourceStages)
                {
                    CheckRecursiveSources(executionFlowDefinition, sourceOfSourceStageName, prohibitedSources);
                }
        }

        private void CreateStageQueueIfNeeded(QueueExecutionFlowStage stage,QueueExecutionFlow executionFlow , QueueExecutionFlowDefinition executionFlowDefinition)
        {
            if(stage.SourceStages != null)
            {
                foreach(var sourceStageName in stage.SourceStages)
                {
                    var sourceStage = executionFlowDefinition.Stages.FindRecord(itm => itm.StageName == sourceStageName);
                    if (sourceStage == null)
                        throw new Exception(String.Format("Source stage '{0}' not found", sourceStageName));
                    CreateStageQueueIfNeeded(sourceStage, executionFlow, executionFlowDefinition);
                }
            }

            string queueName = BuildQueueName(stage, executionFlow);

            StringBuilder queueTitleBuilder = new StringBuilder(stage.QueueTitleTemplate);
            queueTitleBuilder.Replace("#FlowName#", executionFlow.Name);

            var queueSettings = new QueueSettings
            {
                Activator = stage.QueueActivator,
                SingleConcurrentReader = stage.SingleConcurrentReader
            };

            PersistentQueueFactory.Default.CreateQueueIfNotExists(executionFlow.ExecutionFlowId, stage.StageName, stage.QueueItemType.GetQueueItemType().AssemblyQualifiedName,
                queueName, queueTitleBuilder.ToString(), stage.SourceStages, queueSettings);
        }

        private string BuildQueueName(QueueExecutionFlowStage stage, QueueExecutionFlow executionFlow)
        {
            StringBuilder queueNameBuilder = new StringBuilder(stage.QueueNameTemplate);
            queueNameBuilder.Replace("#FlowId", executionFlow.ExecutionFlowId.ToString());
            return  queueNameBuilder.ToString();
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().SetCacheExpired();
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

        public IEnumerable<QueueExecutionFlowInfo> GetExecutionFlows(QueueExecutionFlowFilter filter)
        {
            var cachedFlows = GetCachedQueueExecutionFlows();
            return cachedFlows.MapRecords(QueueExecutionFlowInfoMapper, null);

        }


        public string GetExecutionFlowName(int executionFlowId)
        {
            QueueExecutionFlow executionFlow = GetExecutionFlow(executionFlowId);
            return executionFlow != null ? executionFlow.Name : null;
        }

        public List<QueueExecutionFlow> GetExecutionFlows()
        {
            var cachedExecFlows = GetCachedQueueExecutionFlows();
            if (cachedExecFlows != null)
                return cachedExecFlows.Values.ToList();
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

                      (input.Query.DefinitionId == null || input.Query.DefinitionId.Contains(priceList.DefinitionId)) &&
                      (input.Query.Name == null || priceList.Name.Contains(input.Query.Name));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueExecutionFlows.ToBigResult(input, filterExpression, QueueExecutionFlowMapper));

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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().SetCacheExpired();
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

        private QueueExecutionFlowInfo QueueExecutionFlowInfoMapper(QueueExecutionFlow executionFlow)
        {
            QueueExecutionFlowInfo executionFlowInfo = new QueueExecutionFlowInfo();
            executionFlowInfo.ExecutionFlowId = executionFlow.ExecutionFlowId;
            executionFlowInfo.Name = executionFlow.Name;
            return executionFlowInfo;
        }

        #endregion


    }
}
