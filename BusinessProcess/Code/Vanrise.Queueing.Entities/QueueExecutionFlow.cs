using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlow
    {
        public Guid ExecutionFlowId { get; set; }

        public string Name { get; set; }

        public QueueExecutionFlowTree Tree { get; set; }

        public Guid DefinitionId { get; set; }
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

    public class QueueStageExecutionActivity : BaseExecutionActivity
    {
        public string StageName { get; set; }

        public string QueueName { get; set; }

        public string QueueTitle { get; set; }

        public QueueSettings QueueSettings { get; set; }

        public string QueueTypeFQTN { get; set; }

        public override List<QueueStageInfo> GetQueueStageInfos()
        {
            return new List<QueueStageInfo>
            {
                  new QueueStageInfo 
                  { 
                       QueueTypeFQTN = this.QueueTypeFQTN,
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

    public class ParallelExecutionActivity : CompositeExecutionActivity
    {
        public override List<QueueStageInfo> GetQueueStageInfos()
        {
            //override the parent GetQueueStageInfos to assign the source queue name in order to implement parallel enqueueing
            List<QueueStageInfo> stageInfos = new List<QueueStageInfo>();
            QueueStageInfo firstBranchStage = null;
            foreach (var child in this.Activities)
            {
                List<QueueStageInfo> childStageInfos = child.GetQueueStageInfos();
                if (childStageInfos == null || childStageInfos.Count == 0)
                    throw new Exception("CompositeExecutionActivity doesnt have any child Queue");

                if (firstBranchStage == null)
                    firstBranchStage = childStageInfos[0];
                else
                {
                    var childFirstStage = childStageInfos[0];
                    if (String.Compare(childFirstStage.QueueTypeFQTN, firstBranchStage.QueueTypeFQTN) != 0)
                        throw new Exception(String.Format("First stages in ParallelExecutionActivity should have same QueueTypeFQTN. '{0}' and '{1}' do NOT match", childFirstStage.QueueTypeFQTN, firstBranchStage.QueueTypeFQTN));
                    childFirstStage.SourceQueueStages = new List<QueueStageInfo> { firstBranchStage };
                }
                    
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
