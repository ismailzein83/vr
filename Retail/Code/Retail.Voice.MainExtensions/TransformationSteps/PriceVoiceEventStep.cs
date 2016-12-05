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
        public string VoiceEventPrice { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var voiceChargingManagerVariableName = context.GenerateUniqueMemberName("voiceChargingManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Retail.Voice.Business.VoiceChargingManager();", voiceChargingManagerVariableName);

            var voiceEventPriceVariableName = context.GenerateUniqueMemberName("voiceEventPrice");
            context.AddCodeToCurrentInstanceExecutionBlock("Retail.Voice.Entities.VoiceEventPrice {0} = {1}.PriceVoiceEvent({2},{3},{4},{5},{6},{7});", voiceEventPriceVariableName,
                voiceChargingManagerVariableName, this.AccountId, this.ServiceTypeId, this.RawCDR, this.MappedCDR, this.Duration, this.EventTime);

            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1};", this.VoiceEventPrice, voiceEventPriceVariableName);
        }
    }
}
