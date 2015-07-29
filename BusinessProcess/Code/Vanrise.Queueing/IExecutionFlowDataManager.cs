using System;
namespace Vanrise.Queueing.Data.SQL
{
    public interface IExecutionFlowDataManager
    {
        Vanrise.Queueing.Entities.QueueExecutionFlow GetExecutionFlow(int executionFlowId);
    }
}
