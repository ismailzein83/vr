using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueMappingStep : BaseGenericRuleMappingStep
    {
        public string TargetRecordName { get; set; }

        public string NormalRateFieldName { get; set; }

        public string RatesByRateTypeFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            var ruleTarget = CreateGenericRuleTarget(context);
            var ruleContext = new RateValueRuleContext();
            var ruleManager = new RateValueRuleManager();
            ruleManager.ApplyRateValueRule(ruleContext, this.RuleDefinitionId, ruleTarget);
            var targetRecord = context.GetDataRecord(this.TargetRecordName);
            targetRecord[this.NormalRateFieldName] = ruleContext.NormalRate;
            targetRecord[this.RatesByRateTypeFieldName] = ruleContext.RatesByRateType;
        }
    }
}
