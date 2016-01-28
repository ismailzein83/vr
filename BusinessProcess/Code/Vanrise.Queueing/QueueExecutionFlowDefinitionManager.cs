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
    public class QueueExecutionFlowDefinitionManager
    {
        public IEnumerable<QueueExecutionFlowDefinition> GetExecutionFlowDefinitions()
        {
            var executionFlowDefinitions = GetCachedExecutionFlowDefinitions();
            return executionFlowDefinitions.Values;
        }

       
        public string GetExecutionFlowDefinitionTitle(int definitionID) {

            QueueExecutionFlowDefinition executionFlowDefinition = GetExecutionFlowDefinition(definitionID);
            return executionFlowDefinition != null ? executionFlowDefinition.Title : null;
        }


        public List<QueueExecutionFlowDefinition> GetAll()
        {
            IQueueExecutionFlowDefinitionDataManager manager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            return manager.GetAll();
            
        }



        #region Private Methods

        private Dictionary<int, QueueExecutionFlowDefinition> GetCachedExecutionFlowDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetQueueExecutionFlowDefinitions",
               () =>
               {
                   IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
                   IEnumerable<QueueExecutionFlowDefinition> queueExecutionFlowDefinitions = dataManager.GetAll();
                   return queueExecutionFlowDefinitions.ToDictionary(kvp => kvp.ID, kvp => kvp);
               });
        }


        public QueueExecutionFlowDefinition GetExecutionFlowDefinition(int definitonID)
        {
            var users = GetCachedExecutionFlowDefinitions();
            return users.GetRecord(definitonID);
        }


        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueExecutionFlowDefinitionDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreQueueExecutionFlowDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion

    }
}
