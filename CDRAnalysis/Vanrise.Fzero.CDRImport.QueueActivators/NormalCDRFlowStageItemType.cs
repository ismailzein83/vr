using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.QueueActivators
{
    public class NormalCDRFlowStageItemType : Vanrise.Queueing.Entities.QueueExecutionFlowStageItemType
    {
        public override Type GetQueueItemType()
        {
            return typeof(ImportedCDRBatch);
        }
    }
}
