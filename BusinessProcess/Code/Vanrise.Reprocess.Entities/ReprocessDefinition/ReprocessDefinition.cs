using System;
using System.Collections.Generic;

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
        public List<Guid> SourceRecordStorageIds { get; set; }

        public Guid ExecutionFlowDefinitionId { get; set; }

        public List<string> StageNames { get; set; }

        public List<string> InitiationStageNames { get; set; }

        public List<string> StagesToHoldNames { get; set; }

        public List<string> StagesToProcessNames { get; set; }

        public int RecordCountPerTransaction { get; set; }

        public bool ForceUseTempStorage { get; set; }

        public List<int> IncludedChunkTimes { get; set; }

        public bool CannotBeTriggeredManually { get; set; }

        public PostExecution PostExecution { get; set; }

        //public List<ReprocessDefinitionStage> Stages { get; set; }

        public ReprocessFilterDefinition FilterDefinition { get; set; }
    }

    public class PostExecution
    {
        public List<Guid> ReprocessDefinitionIds { get; set; }
    }

    //public class ReprocessDefinitionStage
    //{
    //    public string StageName { get; set; }

    //    /// <summary>
    //    /// this property is set based on the Stage's QueueActivator. its UI is defined in the QueueActivatorConfig class, ReprocessEditor property
    //    /// </summary>
    //    public ReprocessDefinitionStageExtendedSettings ExtendedSettings { get; set; }
    //}

    //public abstract class ReprocessDefinitionStageExtendedSettings
    //{
    //}
}