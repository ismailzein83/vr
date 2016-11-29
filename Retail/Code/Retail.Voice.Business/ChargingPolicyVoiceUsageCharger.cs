﻿using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Business
{
    public class ChargingPolicyVoiceUsageCharger : IPackageVoiceUsageCharger
    {
        int _chargingPolicyId;
        public ChargingPolicyVoiceUsageCharger(int chargingPolicyId)
        {
            _chargingPolicyId = chargingPolicyId;
        }

        static VoiceChargingManager s_voiceChargingManager = new VoiceChargingManager();

        public void TryChargeVoiceEvent(IVoiceUsageChargerContext context)
        {
            VoiceEventPricingInfo pricingInfo = s_voiceChargingManager.ApplyChargingPolicyToVoiceEvent(this._chargingPolicyId, context.ServiceTypeId, context.RawCDR, context.MappedCDR, context.Duration, context.EventTime);
            if (pricingInfo != null)
            {
                context.PricedPartInfos = new List<VoiceEventPricedPart>
                {
                    new VoiceEventPricedPart
                    {
                        UsageChargingPolicyId = _chargingPolicyId,
                        PricedDuration = context.Duration,
                        Rate = pricingInfo.Rate,
                        Amount = pricingInfo.Amount,
                        RateTypeId = pricingInfo.RateTypeId,
                        CurrencyId = pricingInfo.CurrencyId
                    }
                };
            }
        }
    }
}
