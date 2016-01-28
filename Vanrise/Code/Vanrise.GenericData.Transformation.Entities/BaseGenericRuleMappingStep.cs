using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public abstract class BaseGenericRuleMappingStep : MappingStep
    {
        public int RuleDefinitionId { get; set; }

        public string EffectiveTimeRecordName { get; set; }

        public List<GenericRuleCriteriaFieldMapping> RuleFieldsMappings { get; set; }

        protected GenericRuleTarget CreateGenericRuleTarget(IMappingStepExecutionContext context)
        {
            if (this.EffectiveTimeRecordName == null)
                throw new ArgumentNullException("EffectiveTimeRecordName");
            var effectiveTimeRecord = context.GetDataRecord(this.EffectiveTimeRecordName);

            Dictionary<string, Object> ruleTargetFieldValues = new Dictionary<string, object>();
            foreach (var ruleFieldMapping in this.RuleFieldsMappings)
            {
                var fieldValue = context.GetDataRecord(ruleFieldMapping.TargetRecordName)[ruleFieldMapping.TargetFieldName];
                if (fieldValue != null)
                    ruleTargetFieldValues.Add(ruleFieldMapping.RuleCriteriaFieldName, fieldValue);
            }

            GenericRuleTarget target = new GenericRuleTarget
            {
                TargetFieldValues = ruleTargetFieldValues,
                EffectiveOn = effectiveTimeRecord.Time
            };
            return target;
        }
    }
}
