using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowManager
    {
        public QueuesByStages GetQueuesByStages(int executionFlowId)
        {
            QueueExecutionFlow executionFlow = new QueueExecutionFlow();
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
                PersistentQueueFactory.Default.CreateQueueIfNotExists(stage.QueueTypeFQTN, queueName, queueTitle, sourceQueueNames, stage.QueueSettings);
                if (queuesByStages.ContainsKey(stage.StageName))
                    throw new Exception(String.Format("Duplicate Stage Names: {0}", stage.StageName));
                queuesByStages.Add(stage.StageName, PersistentQueueFactory.Default.GetQueue(queueName));
            }
            return queuesByStages;
        }

        private string GetQueueName(QueueExecutionFlow executionFlow, QueueStageInfo queueStage)
        {
            return String.Format("ExecutionFlow_{0}_Queue_{1}", executionFlow.ExecutionFlowId, queueStage.QueueName);
        }

        private string GetQueueTitle(QueueExecutionFlow executionFlow, QueueStageInfo queueStage)
        {
            return String.Format("{0} - {1}", executionFlow.Name, queueStage.QueueTitle);
        }
    }
}
