using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class ExtraChargeMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("5d6bbc8b-a602-4a94-ae85-8a602ca26805"); } }

        public string InitialRate { get; set; }
        public string EffectiveRate { get; set; }
        public string ExtraChargeRate { get; set; }
        public string CurrencyId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.ExtraChargeRuleContext();", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.TargetTime = {1};", ruleContextVariableName, this.EffectiveTime);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Rate = {1};", ruleContextVariableName, this.InitialRate);

            if (this.CurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.DestinationCurrencyId = {1};", ruleContextVariableName, this.CurrencyId);

            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Pricing.ExtraChargeRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyExtraChargeRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.Rate;", this.EffectiveRate, ruleContextVariableName);

            if (ExtraChargeRate != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.ExtraChargeRate;", this.ExtraChargeRate, ruleContextVariableName);

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
