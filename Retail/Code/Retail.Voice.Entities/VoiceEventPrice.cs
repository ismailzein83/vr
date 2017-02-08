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

        public Decimal? Rate { get; set; }

        public Decimal? Amount { get; set; }

        public int? RateTypeId { get; set; }

        public int CurrencyId { get; set; }

        public List<VoiceEventPricedPart> VoiceEventPricedParts { get; set; }
    }

    public class VoiceEventPricedPart
    {
        public int PackageId { get; set; }

        public int? UsageChargingPolicyId { get; set; }

        public Decimal PricedDuration { get; set; }

        public Decimal? Rate { get; set; }

        public Decimal? Amount { get; set; }

        public int? RateTypeId { get; set; }

        public int CurrencyId { get; set; }
    }
}
