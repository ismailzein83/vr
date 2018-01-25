using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation;

namespace Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators
{
    public class DataTransformationPolicyEvaluator : VoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("6A47048F-BEED-45C5-A6C1-19731B407CAF"); }
        }

        public Guid DataTransformationId { get; set; }

        static DataTransformer s_transformer = new DataTransformer();

        public override void ApplyChargingPolicyToVoiceEvent(IVoiceChargingPolicyEvaluatorContext context)
        {

            var output = s_transformer.ExecuteDataTransformation(this.DataTransformationId, (dtContext) =>
            {
                dtContext.SetRecordValue("MappedCDR", context.MappedCDR);
                dtContext.SetRecordValue("Duration", context.Duration);
                dtContext.SetRecordValue("EventTime", context.EventTime);
                dtContext.SetRecordValue("ChargingPolicyId", context.ChargingPolicyId);
            });
            VoiceEventPricingInfo pricingInfo = output.GetRecordValue("EventPricingInfo");
            pricingInfo.ChargingPolicyId = context.ChargingPolicyId;
            context.EventPricingInfo = pricingInfo;
        }
    }
}
