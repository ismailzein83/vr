using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.Entities
{
    public class ReprocessDefinition
    {
        public Guid ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public ReprocessDefinitionSettings Settings { get; set; }
    }

    public class ReprocessDefinitionSettings
    {
        public Guid SourceRecordStorageId { get; set; }

        public Guid ExecutionFlowDefinitionId { get; set; }

        public List<string> StageNames { get; set; }

        public List<string> InitiationStageNames { get; set; }
    }
}
