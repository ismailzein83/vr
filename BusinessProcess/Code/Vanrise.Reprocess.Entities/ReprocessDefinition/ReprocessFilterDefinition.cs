using System;
using System.Collections.Generic;

namespace Vanrise.Reprocess.Entities
{
    public abstract class ReprocessFilterDefinition
    {
        public abstract Guid ConfigId { get; }

        public bool ApplyFilterToSourceData { get; set; }

        public virtual string RuntimeEditor { get; set; }

        public Dictionary<Guid, Dictionary<string, string>> MappingFields { get; set; }

        public abstract Vanrise.GenericData.Entities.RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context);
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

    public class GenericReprocessFilterFieldDefinition
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; set; }
    }
}
