﻿using System;
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

        Guid AccountBEDefinitionId { get; }

        long PackageAccountId { get; }

        VoiceEventPricingInfo EventPricingInfo { set; }
    }

    public class VoiceEventPricingInfo
    {
        public int ChargingPolicyId { get; set; }

        public Decimal? SaleRate { get; set; }

        public Decimal? SaleAmount { get; set; }

        public int? SaleRateTypeId { get; set; }

        public int SaleCurrencyId { get; set; }

        public Decimal? SaleDurationInSeconds { get; set; }

        public int? SaleRateValueRuleId {get; set;}

        public int? SaleRateTypeRuleId { get; set; }

        public int? SaleTariffRuleId { get; set; }

        public int? SaleExtraChargeRuleId { get; set; }
    }
}
