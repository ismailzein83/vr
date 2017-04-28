using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlowStage
    {
        public string StageName { get; set; }

        public string QueueNameTemplate { get; set; }

        public string QueueTitleTemplate { get; set; }

        public QueueExecutionFlowStageItemType QueueItemType { get; set; }

        public QueueActivator QueueActivator { get; set; }

        public bool IsSequential { get; set; }

        public int? MaximumConcurrentReaders { get; set; }

        public List<string> SourceStages { get; set; }
    }

    public abstract class QueueExecutionFlowStageItemType
    {
        public abstract Type GetQueueItemType();
    }

    public class QueueExecutionFlowStagesByStageName : Dictionary<string, QueueExecutionFlowStage>
    {

    }
}
