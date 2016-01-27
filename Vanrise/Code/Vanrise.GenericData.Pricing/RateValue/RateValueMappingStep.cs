using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueMappingStep : MappingStep
    {
        public int RuleDefinitionId { get; set; }

        public string EffectiveTimeRecordName { get; set; }

        public string TargetRecordName { get; set; }

        public string NormalRateFieldName { get; set; }

        public string RatesByRateTypeFieldName { get; set; }

        public override void Execute(IMappingStepExecutionContext context)
        {
            if (this.EffectiveTimeRecordName == null)
                throw new ArgumentNullException("EffectiveTimeRecordName");
            var effectiveTimeRecord = context.GetDataRecord(this.EffectiveTimeRecordName);
            
            var target = new GenericRuleTarget
            {
                DataRecords = context.GetAllDataRecords(),
                EffectiveOn = effectiveTimeRecord.Time
            };
            var ruleContext = new RateValueRuleContext();
            var ruleManager = new RateValueRuleManager();
            ruleManager.ApplyRateValueRule(ruleContext, this.RuleDefinitionId, target);
            var targetRecord = context.GetDataRecord(this.TargetRecordName);
            targetRecord[this.NormalRateFieldName] = ruleContext.NormalRate;
            targetRecord[this.RatesByRateTypeFieldName] = ruleContext.RatesByRateType;
        }
    }
}
