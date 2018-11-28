using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class CompositeRecordConditionDefinitionGroup : CriteriaDefinition
    {
        public static Guid s_configId = new Guid("0FD33E6D-A578-4E38-81B6-849AFFA535A6");
        public override Guid ConfigId { get { return s_configId; } }

        public List<CompositeRecordConditionDefinition> CompositeRecordFilterDefinitions { get; set; }
    }
}