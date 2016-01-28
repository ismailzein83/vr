using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeMappingStep : BaseGenericRuleMappingStep
    {
        public string SourceRecordName { get; set; }

        public string NormalRateFieldName { get; set; }

        public string RatesByRateTypeFieldName { get; set; }

        public string TargetRecordName { get; set; }

        public string EffectiveRateFieldName { get; set; }

        public string RateTypeFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            var ruleTarget = CreateGenericRuleTarget(context);
            var sourceRecord = context.GetDataRecord(this.SourceRecordName);
            var ruleContext = new RateTypeRuleContext
            {
                NormalRate = sourceRecord.GetFieldValue<Decimal>(this.NormalRateFieldName),
                RatesByRateType = sourceRecord.GetFieldValue<Dictionary<int, Decimal>>(this.RatesByRateTypeFieldName)
            };
            var ruleManager = new RateTypeRuleManager();
            ruleManager.ApplyRateTypeRule(ruleContext, this.RuleDefinitionId, ruleTarget);
            var targetRecord = context.GetDataRecord(this.TargetRecordName);
            targetRecord[this.EffectiveRateFieldName] = ruleContext.EffectiveRate;
            targetRecord[this.RateTypeFieldName] = ruleContext.RateTypeId;
        }
    }
}
