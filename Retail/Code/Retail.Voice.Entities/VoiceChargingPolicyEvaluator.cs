using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public abstract class VoiceChargingPolicyEvaluator
    {
        public abstract Guid ConfigId { get; }

        public abstract void ApplyChargingPolicyToVoiceEvent(IVoiceChargingPolicyEvaluatorContext context);
    }

    public interface IServiceVoiceChargingPolicyEvaluator
    {
        VoiceChargingPolicyEvaluator GetChargingPolicyEvaluator();
    }

    public interface IVoiceChargingPolicyEvaluatorContext
    {
        Guid ServiceTypeId { get; }

        int ChargingPolicyId { get; }

        dynamic RawCDR { get; }

        dynamic MappedCDR { get; }

        Decimal Duration { get; }

        DateTime EventTime { get; }

        VoiceEventPricingInfo EventPricingInfo { set; }
    }

    public class VoiceEventPricingInfo
    {
        public int ChargingPolicyId { get; set; }

        public Decimal Rate { get; set; }

        public Decimal Amount { get; set; }

        public int? RateTypeId { get; set; }

        public int CurrencyId { get; set; }
    }
}
