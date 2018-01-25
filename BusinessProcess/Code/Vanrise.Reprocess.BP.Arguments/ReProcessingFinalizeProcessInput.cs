using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingFinalizeProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public string StageName { get; set; }

        public BatchRecord BatchRecord { get; set; }

        public ReprocessFilter ReprocessFilter { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# for stage: '{0}'. {1}", StageName, BatchRecord.GetBatchTitle());
        }
    }
}