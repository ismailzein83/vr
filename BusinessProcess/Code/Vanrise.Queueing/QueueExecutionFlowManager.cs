using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowManager
    {
        #region Public Methods

        public QueuesByStages GetQueuesByStages(Guid executionFlowId)
        {
            string cacheName = String.Format("QueueExecutionFlowManager_GetQueuesByStages2_{0}", executionFlowId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<ExecutionFlowRuntimeCacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   QueueExecutionFlowManager execFlowManager = new QueueExecutionFlowManager();
                   QueueExecutionFlow executionFlow = execFlowManager.GetExecutionFlow(executionFlowId);

                   if (executionFlow == null)
                       throw new ArgumentNullException(String.Format("Execution Flow '{0}'", executionFlowId));

                   QueueExecutionFlowDefinitionManager definitionManager = new QueueExecutionFlowDefinitionManager();
                   var executionFlowDefinition = definitionManager.GetExecutionFlowDefinition(executionFlow.DefinitionId);
                   if (executionFlowDefinition == null)
                       throw new ArgumentNullException(String.Format("Execution Flow Definition '{0}'", executionFlow.DefinitionId));

                   if (executionFlowDefinition.Stages == null)
                       throw new ArgumentNullException("executionFlowDefinition.Stages");

                   CheckRecursiveSources(executionFlowDefinition);
                   foreach (var stage in executionFlowDefinition.Stages)
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

        private struct GetQueueIdsInSameStageCacheName
        {
            public int QueueId { get; set; }
        }
        public QueueRuntimeInfo GetQueueRuntimeInfo(int queueId)
        {
            QueueRuntimeInfo queueRuntimeInfo = GetQueueRuntimeInfoByQueueId().GetRecord(queueId);
            queueRuntimeInfo.ThrowIfNull("queueRuntimeInfo", queueId);
            return queueRuntimeInfo;
        }

        public Dictionary<int, QueueRuntimeInfo> GetQueueRuntimeInfoByQueueId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<ExecutionFlowRuntimeCacheManager>().GetOrCreateObject("GetQueueRuntimeInfoByQueueId",
               () =>
               {
                   Dictionary<int, QueueRuntimeInfo> rslt = new Dictionary<int, QueueRuntimeInfo>();
                   var allQueues = new QueueInstanceManager().GetReadyQueueInstances();
                   allQueues.ThrowIfNull("allQueues");
                   Dictionary<Guid, QueueExecutionFlowDefinition> execFlowDefs = new QueueExecutionFlowDefinitionManager().GetCachedExecutionFlowDefinitions();
                   execFlowDefs.ThrowIfNull("execFlowDefs");
                   Dictionary<Guid, QueueExecutionFlow> execFlows = GetCachedQueueExecutionFlows();
                   execFlows.ThrowIfNull("execFlows");
                   Dictionary<Guid, List<Guid>> execFlowIdsInSameDefByExecFlowId = new Dictionary<Guid, List<Guid>>();
                   foreach (var execFlow in execFlows.Values)
                   {
                       execFlowIdsInSameDefByExecFlowId.Add(execFlow.ExecutionFlowId, execFlows.Values.Where(itm => itm.DefinitionId == execFlow.DefinitionId).Select(itm => itm.ExecutionFlowId).ToList());
                   }
                   foreach (var queue in allQueues)
                   {
                       QueueRuntimeInfo runtimeInfo = new QueueRuntimeInfo();
                       if(queue.ExecutionFlowId.HasValue)
                       {
                           List<Guid> execFlowIdsInSameDef;
                           if (!execFlowIdsInSameDefByExecFlowId.TryGetValue(queue.ExecutionFlowId.Value, out execFlowIdsInSameDef))
                               throw new NullReferenceException(string.Format("execFlowIdsInSameDef '{0}'", queue.ExecutionFlowId.Value));
                           runtimeInfo.QueueIdsInSameStage = allQueues.Where(itm => itm.ExecutionFlowId.HasValue && execFlowIdsInSameDef.Contains(itm.ExecutionFlowId.Value) && itm.StageName == queue.StageName).Select(itm => itm.QueueInstanceId).ToList();
                           var execFlow = execFlows.GetRecord(queue.ExecutionFlowId.Value);
                           execFlow.ThrowIfNull("execFlow", queue.ExecutionFlowId.Value);
                           var execFlowDefinition = execFlowDefs.GetRecord(execFlow.DefinitionId);
                           execFlowDefinition.ThrowIfNull("execFlowDefinition", execFlow.DefinitionId);
                           execFlowDefinition.Stages.ThrowIfNull("execFlowDefinition.Stages", execFlow.DefinitionId);
                           var matchStage = execFlowDefinition.Stages.FindRecord(itm => itm.StageName == queue.StageName);
                           if (matchStage != null)
                               runtimeInfo.IsSequencial = matchStage.IsSequential;
                       }
                       else
                       {
                           runtimeInfo.QueueIdsInSameStage = new List<int> { queue.QueueInstanceId };
                       }
                       rslt.Add(queue.QueueInstanceId, runtimeInfo);
                   }
                   return rslt;
               });
        }

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDetail> AddExecutionFlow(QueueExecutionFlow executionFlowObj)
        {
            InsertOperationOutput<QueueExecutionFlowDetail> insertOperationOutput = new InsertOperationOutput<QueueExecutionFlowDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            executionFlowObj.ExecutionFlowId = Guid.NewGuid();
            IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
            bool insertActionSucc = dataManager.AddExecutionFlow(executionFlowObj);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(QueueExecutionFlowLoggableEntity.Instance, executionFlowObj);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = QueueExecutionFlowMapper(executionFlowObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public IEnumerable<QueueExecutionFlowInfo> GetExecutionFlowsInfo(QueueExecutionFlowFilter filter)
        {
            var cachedFlows = GetCachedQueueExecutionFlows();
            return cachedFlows.MapRecords(QueueExecutionFlowInfoMapper, null);
        }

        public string GetExecutionFlowName(Guid executionFlowId)
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

        public Vanrise.Entities.IDataRetrievalResult<QueueExecutionFlowDetail> GetFilteredExecutionFlows(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowQuery> input)
        {
            var queueExecutionFlows = GetCachedQueueExecutionFlows();

            Func<QueueExecutionFlow, bool> filterExpression = (priceList) =>

                      (input.Query.DefinitionId == null || input.Query.DefinitionId.Contains(priceList.DefinitionId)) &&
                      (input.Query.Name == null || priceList.Name.Contains(input.Query.Name));

            VRActionLogger.Current.LogGetFilteredAction(QueueExecutionFlowLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueExecutionFlows.ToBigResult(input, filterExpression, QueueExecutionFlowMapper));

        }

        public QueueExecutionFlow GetExecutionFlow(Guid executionFlowId, bool isViewedFromUI)
        {
            var executionFlows = GetCachedQueueExecutionFlows();
            var executionFlowItem = executionFlows.GetRecord(executionFlowId);
            if (executionFlowItem != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(QueueExecutionFlowLoggableEntity.Instance, executionFlowItem);
            return executionFlowItem;
        }

        public QueueExecutionFlow GetExecutionFlow(Guid executionFlowId)
        {
            return GetExecutionFlow(executionFlowId, false);
        }

        public List<QueueExecutionFlow> GetExecutionFlowsByDefinition(Guid executionFlowDefinitionId)
        {
            var executionFlows = GetCachedQueueExecutionFlowsByDefinition();
            return executionFlows.GetRecord(executionFlowDefinitionId);
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
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(QueueExecutionFlowLoggableEntity.Instance, executionFlowObject);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = QueueExecutionFlowMapper(executionFlowObject);
            }
            return updateOperationOutput;

        }

        #endregion

        #region Private Methods

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
                throw new Exception(String.Format("Execution Flow Definition '{0}' has recursive source stages", executionFlowDefinition.Name));
            var sourceStage = executionFlowDefinition.Stages.FindRecord(itm => itm.StageName == sourceStageName);
            if (sourceStage == null)
                throw new Exception(String.Format("Stage '{0}' not found in Execution Flow Definition '{1}'", sourceStageName, executionFlowDefinition.Name));
            prohibitedSources.Add(sourceStage.StageName);
            if (sourceStage.SourceStages != null)
                foreach (var sourceOfSourceStageName in sourceStage.SourceStages)
                {
                    CheckRecursiveSources(executionFlowDefinition, sourceOfSourceStageName, prohibitedSources);
                }
        }

        private void CreateStageQueueIfNeeded(QueueExecutionFlowStage stage, QueueExecutionFlow executionFlow, QueueExecutionFlowDefinition executionFlowDefinition)
        {
            List<string> sourceQueueNames = null;
            if (stage.SourceStages != null && stage.SourceStages.Count > 0)
            {
                sourceQueueNames = new List<string>();
                foreach (var sourceStageName in stage.SourceStages)
                {
                    var sourceStage = executionFlowDefinition.Stages.FindRecord(itm => itm.StageName == sourceStageName);
                    if (sourceStage == null)
                        throw new Exception(String.Format("Source stage '{0}' not found", sourceStageName));
                    sourceQueueNames.Add(BuildQueueName(sourceStage, executionFlow));
                    CreateStageQueueIfNeeded(sourceStage, executionFlow, executionFlowDefinition);
                }
            }

            string queueName = BuildQueueName(stage, executionFlow);

            StringBuilder queueTitleBuilder = new StringBuilder(stage.QueueTitleTemplate);
            queueTitleBuilder.Replace("#FlowName#", executionFlow.Name);
            queueTitleBuilder.Replace("#StageName#", stage.StageName);

            var queueSettings = new QueueSettings
            {
                Activator = stage.QueueActivator,
                MaximumConcurrentReaders = stage.MaximumConcurrentReaders
            };

            PersistentQueueFactory.Default.CreateQueueIfNotExists(executionFlow.ExecutionFlowId, stage.StageName, stage.QueueItemType.GetQueueItemType().AssemblyQualifiedName,
                queueName, queueTitleBuilder.ToString(), sourceQueueNames, queueSettings);
        }

        private string BuildQueueName(QueueExecutionFlowStage stage, QueueExecutionFlow executionFlow)
        {
            StringBuilder queueNameBuilder = new StringBuilder(stage.QueueNameTemplate);
            queueNameBuilder.Replace("#FlowId#", executionFlow.ExecutionFlowId.ToString());
            queueNameBuilder.Replace("#StageName#", stage.StageName);
            return queueNameBuilder.ToString();
        }

        Dictionary<Guid, QueueExecutionFlow> GetCachedQueueExecutionFlows()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("QueueExecutionFlowManager_GetCachedQueueExecutionFlows",
               () =>
               {
                   IQueueExecutionFlowDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
                   return dataManager.GetExecutionFlows().ToDictionary(kvp => kvp.ExecutionFlowId, kvp => kvp);
               });
        }

        Dictionary<Guid, List<QueueExecutionFlow>> GetCachedQueueExecutionFlowsByDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("QueueExecutionFlowManager_GetCachedQueueExecutionFlowsByDefinition",
               () =>
               {
                   Dictionary<Guid, List<QueueExecutionFlow>> queueExecutionFlowsDict = new Dictionary<Guid, List<QueueExecutionFlow>>();
                   Dictionary<Guid, QueueExecutionFlow> queueExecutionFlows = GetCachedQueueExecutionFlows();

                   if (queueExecutionFlows != null)
                   {
                       foreach (var queueExecutionFlow in queueExecutionFlows)
                       {
                           List<QueueExecutionFlow> tempQueueExecutionFlows = queueExecutionFlowsDict.GetOrCreateItem(queueExecutionFlow.Value.DefinitionId);
                           tempQueueExecutionFlows.Add(queueExecutionFlow.Value);
                       }
                   }
                   return queueExecutionFlowsDict;
               });
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueExecutionFlowDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreExecutionFlowsUpdated(ref _updateHandle);
            }
        }

        private class QueueExecutionFlowLoggableEntity : VRLoggableEntityBase
        {
            public static QueueExecutionFlowLoggableEntity Instance = new QueueExecutionFlowLoggableEntity();

            private QueueExecutionFlowLoggableEntity()
            {

            }

            static QueueExecutionFlowManager s_QueueExecutionFlowManager = new QueueExecutionFlowManager();

            public override string EntityUniqueName
            {
                get { return "VR_Queueing_QueueExecutionFlow"; }
            }

            public override string ModuleName
            {
                get { return "Queueing"; }
            }

            public override string EntityDisplayName
            {
                get { return "Queue Execution Flow"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Queueing_QueueExecutionFlow_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                QueueExecutionFlow queueExecutionFlow = context.Object.CastWithValidate<QueueExecutionFlow>("context.Object");
                return queueExecutionFlow.ExecutionFlowId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                QueueExecutionFlow queueExecutionFlow = context.Object.CastWithValidate<QueueExecutionFlow>("context.Object");
                return s_QueueExecutionFlowManager.GetExecutionFlowName(queueExecutionFlow.ExecutionFlowId);
            }
        }

        #endregion

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

    public class QueueRuntimeInfo
    {
        public bool IsSequencial { get; set; }

        public List<int> QueueIdsInSameStage { get; set; }
    }
}
