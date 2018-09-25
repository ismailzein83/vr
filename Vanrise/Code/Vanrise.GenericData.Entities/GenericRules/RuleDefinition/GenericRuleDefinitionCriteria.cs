using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinitionCriteria : CriteriaDefinition
    {
        public static Guid s_configId = new Guid("6B1A593A-E5E5-4CA4-834A-9A20A1FD16BA");
        public override Guid ConfigId { get { return s_configId; } }

        public List<GenericRuleDefinitionCriteriaField> Fields { get; set; }
    }
}
