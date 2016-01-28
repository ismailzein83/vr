using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ExecuteMappingRuleStep : BaseGenericRuleMappingStep
    {
        public string TargetRecordName { get; set; }

        public string TargetFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            GenericRuleTarget ruleTarget = CreateGenericRuleTarget(context);

            MappingRuleManager ruleManager = new MappingRuleManager();
            var rule = ruleManager.GetMatchRule(this.RuleDefinitionId, ruleTarget);
            if (rule != null)
            {
                var targetRecord = context.GetDataRecord(this.TargetFieldName);
                targetRecord[this.TargetFieldName] = rule.Settings.Value;
            }
        }
    }
}
