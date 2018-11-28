using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class CompositeRecordConditionDefinition
    {
        public string Name { get; set; }

        public CompositeRecordConditionDefinitionSettings Settings { get; set; }
    }

    public abstract class CompositeRecordConditionDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void GetFields(ICompositeRecordConditionDefinitionSettingsGetFieldsContext context);
    }

    public interface ICompositeRecordConditionDefinitionSettingsGetFieldsContext
    {
        Dictionary<string, DataRecordField> Fields { set; }
    }

    public class CompositeRecordConditionDefinitionSettingsGetFieldsContext : ICompositeRecordConditionDefinitionSettingsGetFieldsContext
    {
        public Dictionary<string, DataRecordField> Fields { get; set; }
    }
}