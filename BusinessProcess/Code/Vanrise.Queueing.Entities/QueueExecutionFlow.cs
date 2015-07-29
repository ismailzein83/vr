using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlow
    {
        public int ExecutionFlowId { get; set; }

        public string Name { get; set; }

        public QueueExecutionFlowTree Tree { get; set; }

        public int DefinitionId { get; set; }
    }

    public class QueueExecutionFlowTree : CompositeExecutionActivity
    {

    }

    public abstract class BaseExecutionActivity
    {
        public abstract List<QueueStageInfo> GetQueueStageInfos();
    }

    public class QueueStageInfo
    {
        public string QueueTypeFQTN { get; set; }

        public string StageName { get; set; }

        public string QueueName { get; set; }

        public string QueueTitle { get; set; }

        public QueueSettings QueueSettings { get; set; }

        public List<QueueStageInfo> SourceQueueStages { get; set; }
    }

    public class QueueStageExecutionActivity<T> : BaseExecutionActivity
    {
        public string StageName { get; set; }

        public string QueueName { get; set; }

        public string QueueTitle { get; set; }

        public QueueSettings QueueSettings { get; set; }

        public override List<QueueStageInfo> GetQueueStageInfos()
        {
            return new List<QueueStageInfo>
            {
                  new QueueStageInfo 
                  { 
                       QueueTypeFQTN = typeof(T).AssemblyQualifiedName,
                       StageName = this.StageName,
                       QueueName = this.QueueName,
                       QueueTitle = !String.IsNullOrEmpty(this.QueueTitle) ? this.QueueTitle : this.QueueName,
                       QueueSettings = this.QueueSettings
                  }
            };
        }
    }

    public class SequenceExecutionActivity : CompositeExecutionActivity
    {
        
    }

    public class ParallelExecutionActivity<T> : CompositeExecutionActivity
    {
        public override List<QueueStageInfo> GetQueueStageInfos()
        {
            //override the parent GetQueueStageInfos to assign the source queue name in order to implement parallel enqueueing
            List<QueueStageInfo> stageInfos = new List<QueueStageInfo>();
            QueueStageInfo firstStage = null;
            foreach (var child in this.Activities)
            {
                List<QueueStageInfo> childStageInfos = child.GetQueueStageInfos();
                if (childStageInfos == null || childStageInfos.Count == 0)
                    throw new Exception("CompositeExecutionActivity doesnt have any child Queue");

                if (firstStage == null)
                    firstStage = childStageInfos[0];
                else
                    childStageInfos[0].SourceQueueStages = new List<QueueStageInfo> { firstStage };
                stageInfos.AddRange(childStageInfos);
            }
            return stageInfos;
        }
    }

    public class SplitExecutionActivity : CompositeExecutionActivity
    {
        
    }

    public abstract class CompositeExecutionActivity : BaseExecutionActivity
    {
        public List<BaseExecutionActivity> Activities { get; set; }

        public override List<QueueStageInfo> GetQueueStageInfos()
        {
            List<QueueStageInfo> stageInfos = new List<QueueStageInfo>();
            foreach (var child in this.Activities)
            {
                List<QueueStageInfo> childStageInfos = child.GetQueueStageInfos();
                if (childStageInfos == null || childStageInfos.Count == 0)
                    throw new Exception("CompositeExecutionActivity doesnt have any queue stages");
                stageInfos.AddRange(childStageInfos);
            }
            return stageInfos;
        }
    }
}
