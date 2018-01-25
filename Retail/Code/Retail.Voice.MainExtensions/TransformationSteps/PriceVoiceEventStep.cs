using System;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Voice.MainExtensions.TransformationSteps
{
    public class PriceVoiceEventStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("FB2D7DC4-AF79-4068-8452-1058AF7544D7"); } }

        //Input Fields
        public string AccountBEDefinitionID { get; set; }
        public string AccountId { get; set; }
        public string ServiceTypeId { get; set; }
        public string MappedCDR { get; set; }
        public string Duration { get; set; }
        public string EventTime { get; set; }

        //Output Fields
        public string PackageId { get; set; }
        public string UsageChargingPolicyId { get; set; }
        public string SaleDurationInSeconds { get; set; }
        public string SaleRate { get; set; }
        public string SaleAmount { get; set; }
        public string SaleRateTypeId { get; set; }
        public string SaleCurrencyId { get; set; }
        public string VoiceEventPricedParts { get; set; }
        public string SaleRateValueRuleId { get; set; }
        public string SaleRateTypeRuleId { get; set; }
        public string SaleTariffRuleId { get; set; }
        public string SaleExtraChargeRuleId { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var voiceChargingManagerVariableName = context.GenerateUniqueMemberName("voiceChargingManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Business.VoiceChargingManager();", voiceChargingManagerVariableName);

            var voiceEventPriceVariableName = context.GenerateUniqueMemberName("voiceEventPrice");
            context.AddCodeToCurrentInstanceExecutionBlock("Retail.Voice.Entities.VoiceEventPrice {0} = {1}.PriceVoiceEvent({2},{3},{4},{5},{6},{7});", voiceEventPriceVariableName,
                voiceChargingManagerVariableName, this.AccountBEDefinitionID, this.AccountId, this.ServiceTypeId, this.MappedCDR, this.Duration, this.EventTime);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", voiceEventPriceVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            if (this.PackageId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.PackageId;", this.PackageId, voiceEventPriceVariableName);

            if (this.UsageChargingPolicyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.UsageChargingPolicyId;", this.UsageChargingPolicyId, voiceEventPriceVariableName);

            if (this.SaleDurationInSeconds != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleDurationInSeconds;", this.SaleDurationInSeconds, voiceEventPriceVariableName);

            if (this.SaleRate != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleRate;", this.SaleRate, voiceEventPriceVariableName);

            if (this.SaleAmount != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleAmount;", this.SaleAmount, voiceEventPriceVariableName);

            if (this.SaleRateTypeId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleRateTypeId;", this.SaleRateTypeId, voiceEventPriceVariableName);

            if (this.SaleCurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleCurrencyId;", this.SaleCurrencyId, voiceEventPriceVariableName);

            if (this.SaleRateValueRuleId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleRateValueRuleId;", this.SaleRateValueRuleId, voiceEventPriceVariableName);

            if (this.SaleRateTypeRuleId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleRateTypeRuleId;", this.SaleRateTypeRuleId, voiceEventPriceVariableName);

            if (this.SaleTariffRuleId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleTariffRuleId;", this.SaleTariffRuleId, voiceEventPriceVariableName);

            if (this.SaleExtraChargeRuleId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.SaleExtraChargeRuleId;", this.SaleExtraChargeRuleId, voiceEventPriceVariableName);

            if (this.VoiceEventPricedParts != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.VoiceEventPricedParts;", this.VoiceEventPricedParts, voiceEventPriceVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
