using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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

        public List<string> StagesToHoldNames { get; set; }

        public List<string> StagesToProcessNames { get; set; }

        public int RecordCountPerTransaction { get; set; }

        //public List<ReprocessDefinitionStage> Stages { get; set; }

        //public ReprocessFilterDefinition FilterDefinition { get; set; }

        //public bool ApplyFilterToSourceData { get; set; }

        //public List<ReprocessFilterFieldMapping> SourceDataFilterFieldMappings { get; set; }
    }

    //public abstract class ReprocessFilterDefinition
    //{
    //    public abstract Guid ConfigId { get; }

    //    public virtual string RuntimeEditor { get; set; }

    //    public abstract Vanrise.GenericData.Entities.RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context);
    //}

    //public abstract class ReprocessFilter
    //{

    //}

    //public interface IReprocessFilterGetFilterGroupContext
    //{
    //    ReprocessFilter ReprocessFilter { get; }
    //}

    //public class ReprocessFilterFieldMapping
    //{
    //    public string FilterFieldName { get; set; }

    //    public string MappedFieldName { get; set; }
    //}

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

    //public class FilterGroupReprocessFilterDefinition : ReprocessFilterDefinition
    //{
    //    public override Guid ConfigId
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public List<FilterGroupReprocessFilterFieldDefinition> Fields { get; set; }

    //    public override GenericData.Entities.RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context)
    //    {
    //        FilterGroupReprocessFilter recordFilterReprocessFilter = context.ReprocessFilter.CastWithValidate<FilterGroupReprocessFilter>("context.ReprocessFilter");
    //        return recordFilterReprocessFilter.FilterGroup;
    //    }
    //}

    //public class FilterGroupReprocessFilter : ReprocessFilter
    //{
    //    public GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
    //}

    //public class FilterGroupReprocessFilterFieldDefinition
    //{
    //    public string Name { get; set; }

    //    public string Title { get; set; }

    //    public Vanrise.GenericData.Entities.DataRecordFieldType Type { get; set; }
    //}
}
