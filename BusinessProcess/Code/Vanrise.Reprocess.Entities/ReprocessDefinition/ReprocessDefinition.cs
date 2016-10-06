using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Reprocess.Entities
{
    public enum ChunkTime
    {
        [ChunkTimeAttribute(5)]
        FiveMinutes = 0,
        [ChunkTimeAttribute(10)]
        TenMinutes = 1,
        [ChunkTimeAttribute(15)]
        FifteenMinutes = 2,
        [ChunkTimeAttribute(30)]
        ThirtyMinutes = 3,
        [ChunkTimeAttribute(60)]
        OneHour = 4,
        [ChunkTimeAttribute(120)]
        TwoHours = 5,
        [ChunkTimeAttribute(180)]
        ThreeHours = 6,
        [ChunkTimeAttribute(360)]
        SixHours = 7
    }

    public class ChunkTimeAttribute : Attribute
    {
        public ChunkTimeAttribute(int value)
        {
            this.Value = value;
        }
        public int Value { get; set; }

    }

    public class ReprocessDefinition
    {
        public int ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public ReprocessDefinitionSettings Settings { get; set; }
    }

    public class ReprocessDefinitionSettings
    {
        public Guid SourceRecordStorageId { get; set; }

        public int ExecutionFlowDefinitionId { get; set; }

        public List<string> StageNames { get; set; }

        public List<string> InitiationStageNames { get; set; }
    }
}
