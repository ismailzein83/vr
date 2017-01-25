using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Voice.MainExtensions.TransformationSteps
{
    public class PriceVoiceEventStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("FB2D7DC4-AF79-4068-8452-1058AF7544D7"); } }

        //Input Fields
        public string AccountId { get; set; }
        public string ServiceTypeId { get; set; }
        public string RawCDR { get; set; }
        public string MappedCDR { get; set; }
        public string Duration { get; set; }
        public string EventTime { get; set; }

        //Output Fields
        public string PackageId { get; set; }
        public string UsageChargingPolicyId { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
        public string RateTypeId { get; set; }
        public string CurrencyId { get; set; }
        public string VoiceEventPricedParts { get; set; }
        public string AccountBEDefinitionID { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var voiceChargingManagerVariableName = context.GenerateUniqueMemberName("voiceChargingManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Business.VoiceChargingManager();", voiceChargingManagerVariableName);

            var voiceEventPriceVariableName = context.GenerateUniqueMemberName("voiceEventPrice");
            context.AddCodeToCurrentInstanceExecutionBlock("Retail.Voice.Entities.VoiceEventPrice {0} = {1}.PriceVoiceEvent({2},{3},{4},{5},{6},{7},{8});", voiceEventPriceVariableName,
                voiceChargingManagerVariableName,this.AccountBEDefinitionID, this.AccountId, this.ServiceTypeId, this.RawCDR, this.MappedCDR, this.Duration, this.EventTime);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", voiceEventPriceVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            if (this.PackageId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.PackageId;", this.PackageId, voiceEventPriceVariableName);

            if (this.UsageChargingPolicyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.UsageChargingPolicyId;", this.UsageChargingPolicyId, voiceEventPriceVariableName);

            if (this.Rate != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.Rate;", this.Rate, voiceEventPriceVariableName);

            if (this.Amount != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.Amount;", this.Amount, voiceEventPriceVariableName);

            if (this.RateTypeId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.RateTypeId;", this.RateTypeId, voiceEventPriceVariableName);

            if (this.CurrencyId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.CurrencyId;", this.CurrencyId, voiceEventPriceVariableName);

            if (this.VoiceEventPricedParts != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.VoiceEventPricedParts;", this.VoiceEventPricedParts, voiceEventPriceVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
