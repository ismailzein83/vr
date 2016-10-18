using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueExecutionFlowDefinitionDataManager : IDataManager
    {
        List<QueueExecutionFlowDefinition> GetAll();

        bool UpdateExecutionFlowDefinition(Vanrise.Queueing.Entities.QueueExecutionFlowDefinition executionFlowDefinitionObject);

        bool AreQueueExecutionFlowDefinitionUpdated(ref object updateHandle);

        bool AddExecutionFlowDefinition(Vanrise.Queueing.Entities.QueueExecutionFlowDefinition executionFlowDefinition);
    }
}
