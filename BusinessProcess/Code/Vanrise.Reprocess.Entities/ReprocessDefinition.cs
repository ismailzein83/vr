using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.Entities
{
    public class ReprocessDefinition
    {
        public int ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public int SourceRecordStorageId { get; set; }

        public int ExecutionFlowDefinitionId { get; set; }

        public List<string> InitiationStageNames { get; set; }
    }
}
