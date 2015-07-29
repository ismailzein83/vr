using System;
namespace Vanrise.Queueing.Data
{
    public interface IQueueExecutionFlowDataManager : IDataManager
    {
        Vanrise.Queueing.Entities.QueueExecutionFlow GetExecutionFlow(int executionFlowId);
    }
}
