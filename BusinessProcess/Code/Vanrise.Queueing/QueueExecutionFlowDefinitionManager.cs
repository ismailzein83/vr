﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Entities;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowDefinitionManager
    {

       

        public IEnumerable<QueueExecutionFlowDefinitionInfo> GetExecutionFlowDefinitions(QueueExecutionFlowDefinitionFilter filter)
        {
            var executionFlowDefinitions = GetCachedExecutionFlowDefinitions();
            return executionFlowDefinitions.MapRecords(QueueExecutionFlowDefinitionInfoMapper, null);
        }


        public string GetExecutionFlowDefinitionTitle(int definitionID)
        {

            QueueExecutionFlowDefinition executionFlowDefinition = GetExecutionFlowDefinition(definitionID);
            return executionFlowDefinition != null ? executionFlowDefinition.Title : null;
        }
        
        public List<QueueExecutionFlowDefinition> GetAll()
        {
            var cachedFlowDefinitions = GetCachedExecutionFlowDefinitions();
            if (cachedFlowDefinitions != null)
                return cachedFlowDefinitions.Values.ToList();
            else
                return null;
        }

        public QueueExecutionFlowStagesByStageName GetFlowStages(int definitionId)
        {
            String cacheName = String.Format("GetFlowStages_{0}", definitionId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var execFlowDefinition = GetExecutionFlowDefinition(definitionId);
                    if (execFlowDefinition == null)
                        return null;
                    QueueExecutionFlowStagesByStageName stagesByName = new QueueExecutionFlowStagesByStageName();
                    if (execFlowDefinition.Stages != null)
                    {
                        foreach(var stage in execFlowDefinition.Stages)
                        {
                            stagesByName.Add(stage.StageName, stage);
                        }
                    }
                    return stagesByName;
                });
        }

        public Vanrise.Entities.IDataRetrievalResult<QueueExecutionFlowDefinitionDetail> GetFilteredExecutionFlowDefinitions(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowDefinitionQuery> input)
        {
            var queueExecutionFlowDefinitions = GetCachedExecutionFlowDefinitions();

            Func<QueueExecutionFlowDefinition, bool> filterExpression = (executionFlowDefinition) =>

                      (input.Query.Title == null || executionFlowDefinition.Title.Contains(input.Query.Title)) &&
                      (input.Query.Name == null || executionFlowDefinition.Name.Contains(input.Query.Name));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueExecutionFlowDefinitions.ToBigResult(input, filterExpression, QueueExecutionFlowDefinitionDetailMapper));

        }


        public QueueExecutionFlowDefinition GetExecutionFlowDefinition(int definitonID)
        {
            var users = GetCachedExecutionFlowDefinitions();
            return users.GetRecord(definitonID);
        }


        public Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDefinitionDetail> UpdateExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinitionObject)
        {
            IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateExecutionFlowDefinition(executionFlowDefinitionObject);
            UpdateOperationOutput<QueueExecutionFlowDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = QueueExecutionFlowDefinitionDetailMapper(executionFlowDefinitionObject);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }

        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDefinitionDetail> AddExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinitionObj)
        {
            InsertOperationOutput<QueueExecutionFlowDefinitionDetail> insertOperationOutput = new InsertOperationOutput<QueueExecutionFlowDefinitionDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int executionFlowDefinitionId = -1;

            IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddExecutionFlowDefinition(executionFlowDefinitionObj, out executionFlowDefinitionId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                executionFlowDefinitionObj.ID = executionFlowDefinitionId;
                insertOperationOutput.InsertedObject = QueueExecutionFlowDefinitionDetailMapper(executionFlowDefinitionObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }


        #region Private Methods

        private Dictionary<int, QueueExecutionFlowDefinition> GetCachedExecutionFlowDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedExecutionFlowDefinitions",
               () =>
               {
                   IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
                   IEnumerable<QueueExecutionFlowDefinition> queueExecutionFlowDefinitions = dataManager.GetAll();
                   return queueExecutionFlowDefinitions.ToDictionary(kvp => kvp.ID, kvp => kvp);
               });
        }


       

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueExecutionFlowDefinitionDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return this.IsCacheExpired();
            }

            public bool IsCacheExpired()
            {
                return _dataManager.AreQueueExecutionFlowDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion


        #region Mappers

        private QueueExecutionFlowDefinitionInfo QueueExecutionFlowDefinitionInfoMapper(QueueExecutionFlowDefinition queueExecutionFlowDefinition)
        {
            QueueExecutionFlowDefinitionInfo queueExecutionFlowDefinitionInfo = new QueueExecutionFlowDefinitionInfo();
            queueExecutionFlowDefinitionInfo.ID = queueExecutionFlowDefinition.ID;
            queueExecutionFlowDefinitionInfo.Title = queueExecutionFlowDefinition.Title;
            return queueExecutionFlowDefinitionInfo;

        }

        private QueueExecutionFlowDefinitionDetail QueueExecutionFlowDefinitionDetailMapper(QueueExecutionFlowDefinition executionFlowDefinition)
        {
            QueueExecutionFlowDefinitionDetail executionFlowDefinitionDetail = new QueueExecutionFlowDefinitionDetail();
            executionFlowDefinitionDetail.Entity = executionFlowDefinition;
            return executionFlowDefinitionDetail;
        }

        #endregion

    }
}
