using System;
using System.Collections.Generic;
namespace Vanrise.Queueing.Data
{
    public interface IQueueExecutionFlowDataManager : IDataManager
    {
        List<Vanrise.Queueing.Entities.QueueExecutionFlow> GetExecutionFlows();

        bool AddExecutionFlow(Vanrise.Queueing.Entities.QueueExecutionFlow executionFlow);

        bool AreExecutionFlowsUpdated(ref object updateHandle);


        bool UpdateExecutionFlow(Vanrise.Queueing.Entities.QueueExecutionFlow executionFlowObject);
    }
}
