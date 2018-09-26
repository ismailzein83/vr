using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleQuery
    {
        public string ReportName { get; set; }
        public Guid RuleDefinitionId { get; set; }

        public string Description { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public Dictionary<string, object> CriteriaFieldValues { get; set; }

        public object SettingsFilterValue { get; set; }
    }
}