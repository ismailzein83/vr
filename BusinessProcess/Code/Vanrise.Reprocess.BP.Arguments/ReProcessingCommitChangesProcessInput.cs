using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingCommitChangesProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public string StageName { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public override string GetTitle()
        {
            return String.Format("Stage: {0}. From: {1} - To: {2}", StageName, From.ToString(), To.ToString());
        }
    }
}
