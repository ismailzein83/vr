using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.RA.Business
{
    public class PostpaidTaxMappingStep : BaseGenericRuleMappingStep
    {
        public override Guid ConfigId { get { return new Guid("C96E67E1-F002-484A-9ED6-C876982DB642"); } }

        #region Voice
        public string DurationInSeconds { get; set; }

        public string TotalVoiceAmount { get; set; }

        public string TotalVoiceTaxValue { set; get; }
        #endregion

        #region SMS

        public string NumberOfSMS { get; set; }

        public string TotalSMSAmount { get; set; }

        public string TotalSMSTaxValue { set; get; }
        #endregion

        #region Transaction
        public string TotalTransactionAmount { get; set; }

        public string TotalTransactionTaxValue { set; get; }
        #endregion
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string ruleTargetVariableName;
            base.GenerateRuleTargetExecutionCode<GenericRuleTarget>(context, out ruleTargetVariableName);
            var ruleContextVariableName = context.GenerateUniqueMemberName("ruleContext");
            StringBuilder contextInitialization = new StringBuilder(@"var #VARIABLENAME# = new Retail.RA.Business.PostpaidTaxRuleContext(){
                                                                        VoiceContext = new Retail.RA.Business.PostpaidTaxRuleVoiceContext(),
                                                                        SMSContext = new Retail.RA.Business.PostpaidTaxRuleSMSContext(),
                                                                        TransactionContext = new Retail.RA.Business.PostpaidTaxRuleTransactionContext()
                                                                     };");
            contextInitialization.Replace("#VARIABLENAME#", ruleContextVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock(contextInitialization.ToString());
            var ruleManagerVariableName = context.GenerateUniqueMemberName("ruleManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.RA.Business.PostpaidTaxRuleManager();", ruleManagerVariableName);

            if (!string.IsNullOrEmpty(this.TotalVoiceAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.VoiceContext.TotalAmount = {1};", ruleContextVariableName, this.TotalVoiceAmount);
            if (!string.IsNullOrEmpty(this.DurationInSeconds))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.VoiceContext.DurationInSeconds = {1};", ruleContextVariableName, this.DurationInSeconds);


            if (!string.IsNullOrEmpty(this.TotalSMSAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.SMSContext.TotalAmount = {1};", ruleContextVariableName, this.TotalSMSAmount);

            if (!string.IsNullOrEmpty(this.NumberOfSMS))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.SMSContext.NumberOfSMS = {1};", ruleContextVariableName, this.NumberOfSMS);


            if (!string.IsNullOrEmpty(this.TotalTransactionAmount))
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.TransactionContext.TotalAmount = {1};", ruleContextVariableName, this.TotalTransactionAmount);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.ApplyTaxRule({1}, new Guid(\"{2}\"), {3});",
                ruleManagerVariableName, ruleContextVariableName, this.RuleDefinitionId, ruleTargetVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.Rule != null) ", ruleContextVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{ ");

            if (!string.IsNullOrEmpty(this.TotalVoiceTaxValue))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.VoiceContext.TotalTaxValue;", this.TotalVoiceTaxValue, ruleContextVariableName);

            if (!string.IsNullOrEmpty(this.TotalSMSTaxValue))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SMSContext.TotalTaxValue;", this.TotalSMSTaxValue, ruleContextVariableName);

            if (!string.IsNullOrEmpty(this.TotalTransactionTaxValue))
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.TransactionContext.TotalTaxValue;", this.TotalTransactionTaxValue, ruleContextVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("} ");

            base.SetIdRuleMatched(context, ruleContextVariableName);
        }
    }
}
