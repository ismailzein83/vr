using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ExecuteMappingRuleStep : MappingStep
    {
        public int RuleDefinitionId { get; set; }

        public string EffectiveTimeRecordName { get; set; }

        public string TargetRecordName { get; set; }

        public string TargetFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            if (this.EffectiveTimeRecordName == null)
                throw new ArgumentNullException("EffectiveTimeRecordName");
            var effectiveTimeRecord = context.GetDataRecord(this.EffectiveTimeRecordName);
            MappingRuleManager ruleManager = new MappingRuleManager();
            GenericRuleTarget target = new GenericRuleTarget
            {
                DataRecords = context.GetAllDataRecords(),
                EffectiveOn = effectiveTimeRecord.Time
            };
            var rule = ruleManager.GetMatchRule(this.RuleDefinitionId, target);
            if (rule != null)
            {
                var targetRecord = context.GetDataRecord(this.TargetFieldName);
                targetRecord[this.TargetFieldName] = rule.Settings.Value;
            }
        }
    }
}
