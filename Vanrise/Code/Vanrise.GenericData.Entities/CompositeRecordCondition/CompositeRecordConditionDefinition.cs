using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class CompositeGroupConditionDefinition : CriteriaDefinition
    {
        public static Guid s_configId = new Guid("0FD33E6D-A578-4E38-81B6-849AFFA535A6");
        public override Guid ConfigId { get { return s_configId; } }

        public List<CompositeRecordConditionDefinition> CompositeRecordConditionDefinitions { get; set; }
    }

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