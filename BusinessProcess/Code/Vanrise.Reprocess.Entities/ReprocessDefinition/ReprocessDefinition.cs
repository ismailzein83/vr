using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.Entities
{
    public enum ChunkTime
    {
        FiveMinutes = 5,
        TenMinutes = 10,
        FifteenMinutes = 15,
        ThirtyMinutes = 30,
        OneHour = 60,
        TwoHours = 120,
        ThreeHours = 180,
        SixHours = 360
    }

    public class ReprocessDefinition
    {
        public int ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public ReprocessDefinitionSettings Settings { get; set; }

        public int SourceRecordStorageId { get; set; }

        public int ExecutionFlowDefinitionId { get; set; }

        public List<string> InitiationStageNames { get; set; }
    }

    public class ReprocessDefinitionSettings
    {
        public int SourceRecordStorageId { get; set; }

        public int ExecutionFlowDefinitionId { get; set; }

        public List<string> InitiationStageNames { get; set; }
    }
}
