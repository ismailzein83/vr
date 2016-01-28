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
    public class QueueExecutionFlowDefinitionManager
    {
        public List<QueueExecutionFlowDefinition> GetFilteredExecutionFlowDefinitions()
        {
            IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            return dataManager.GetAll();
        }

        public List<QueueExecutionFlowDefinition> GetAll()
        {
            IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            return dataManager.GetAll();
        }



        public string GetExecutionFlowDefinitionTitle(int definitionID) {

            List<QueueExecutionFlowDefinition> executionFlowDefinitions = GetFilteredExecutionFlowDefinitions();

            QueueExecutionFlowDefinition executionFlowDefinition = executionFlowDefinitions.Where(x => x.ID == definitionID).FirstOrDefault();
            return executionFlowDefinition.Title;
        
        }



    }
}
