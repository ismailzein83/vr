using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public class VoiceEventPrice
    {
        public int? PackageId { get; set; }

        public int? UsageChargingPolicyId { get; set; }

        public Decimal? SaleRate { get; set; }

        public Decimal? SaleAmount { get; set; }

        public int? SaleRateTypeId { get; set; }

        public int SaleCurrencyId { get; set; }

        public Decimal? SaleDurationInSeconds { get; set; }

        public int? SaleRateValueRuleId { get; set; }

        public int? SaleRateTypeRuleId { get; set; }

        public int? SaleTariffRuleId { get; set; }

        public int? SaleExtraChargeRuleId { get; set; }

        public List<VoiceEventPricedPart> VoiceEventPricedParts { get; set; }
    }

    public class VoiceEventPricedPart
    {
        public int PackageId { get; set; }

        public int? UsageChargingPolicyId { get; set; }

        public Decimal PricedDuration { get; set; }

        public Decimal? SaleRate { get; set; }

        public Decimal? SaleAmount { get; set; }

        public int? SaleRateTypeId { get; set; }

        public int SaleCurrencyId { get; set; }

        public Decimal? SaleDurationInSeconds { get; set; }

        public int? SaleRateValueRuleId { get; set; }

        public int? SaleRateTypeRuleId { get; set; }

        public int? SaleTariffRuleId { get; set; }

        public int? SaleExtraChargeRuleId { get; set; }
    }
}
