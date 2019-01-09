﻿using System;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class SMSRateValueMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("24D2417F-ECDE-41E5-8363-892905730662"); } }

        public string NormalRate { get; set; }

        public string RatesByRateType { get; set; }

        public string CurrencyId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.SMSRateValueRuleContext();", ruleContextVariableName);
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.SMSRateValueRuleManager();", ruleManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplySMSRateValueRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) ", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{ ");
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.NormalRate;", this.NormalRate, ruleContextVariableName);

            if (this.CurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CurrencyId;", this.CurrencyId, ruleContextVariableName);

            if (this.RatesByRateType != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RatesByRateType;", this.RatesByRateType, ruleContextVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("} ");

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
