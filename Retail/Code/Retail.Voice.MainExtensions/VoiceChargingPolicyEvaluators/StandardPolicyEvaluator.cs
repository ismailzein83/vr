using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators
{
    public class StandardPolicyEvaluator : VoiceChargingPolicyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("B0B5BC1F-E899-4AE5-AEFB-4FCD5D1BA140"); }
        }

        public Guid? TariffRuleDefinitionId { get; set; }
        public Guid? RateValueRuleDefinitionId { get; set; }
        public Guid? RateTypeRuleDefinitionId { get; set; }
        public Guid? ExtraChargeRuleDefinitionId { get; set; }


        public override void ApplyChargingPolicyToVoiceEvent(IVoiceChargingPolicyEvaluatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
