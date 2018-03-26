using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Reprocess.Entities
{
    public class ReprocessDefinition
    {
        public Guid ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public ReprocessDefinitionSettings Settings { get; set; }
    }

    public class PostExecution
    {
        public List<Guid> ReprocessDefinitionIds { get; set; }
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

        public PostExecution PostExecution { get; set; }

        //public List<ReprocessDefinitionStage> Stages { get; set; }

        public ReprocessFilterDefinition FilterDefinition { get; set; }
    }

    public abstract class ReprocessFilterDefinition
    {
        public abstract Guid ConfigId { get; }

        public bool ApplyFilterToSourceData { get; set; }

        public virtual string RuntimeEditor { get; set; }

        public Dictionary<Guid, Dictionary<string, string>> MappingFields { get; set; }

        public abstract Vanrise.GenericData.Entities.RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context);
    }

    public abstract class ReprocessFilter
    {

    }

    public interface IReprocessFilterGetFilterGroupContext
    {
        Guid? TargetDataRecordTypeId { get; }

        ReprocessFilter ReprocessFilter { get; }
    }

    public class ReprocessFilterGetFilterGroupContext : IReprocessFilterGetFilterGroupContext
    {
        public Guid? TargetDataRecordTypeId { get; set; }
        public ReprocessFilter ReprocessFilter { get; set; }
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

    

    public class GenericReprocessFilter : ReprocessFilter
    {
        public Dictionary<string, List<object>> Fields { get; set; }

        public RecordQueryLogicalOperator LogicalOperator { get; set; }
    }

    public class GenericReprocessFilterFieldDefinition
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; set; }
    }

    public class ReprocessFilterDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Reprocess_ReprocessFilterDefinitionConfig";

        public string Editor { get; set; }
    }
}
