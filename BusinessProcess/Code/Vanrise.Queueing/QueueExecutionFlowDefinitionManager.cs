using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowDefinitionManager
    {
        public List<QueueExecutionFlowDefinition> GetAll()
        {
            IQueueExecutionFlowDefinitionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDefinitionDataManager>();
            return dataManager.GetAll();
        }

        public string GetExecutionFlowDefinitionName(int definitionID) {

            List<QueueExecutionFlowDefinition> allExecutionFlowDefinitions = GetAll();
            QueueExecutionFlowDefinition executionFlowDefinitionItem = allExecutionFlowDefinitions.Where(x => x.ID == definitionID).FirstOrDefault();
            return executionFlowDefinitionItem.Title;
        
        }

    }
}
