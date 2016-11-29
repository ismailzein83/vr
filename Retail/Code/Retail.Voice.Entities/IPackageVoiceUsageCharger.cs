using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public interface IPackageVoiceUsageCharger
    {
        void TryChargeVoiceEvent(IVoiceUsageChargerContext context);
    }

    public interface IPackageSettingVoiceUsageCharger
    {
        bool TryGetVoiceUsageCharger(Guid serviceTypeId, out IPackageVoiceUsageCharger voiceUsageCharging);
    }

    public interface IVoiceUsageChargerContext
    {
        Guid ServiceTypeId { get; }

        dynamic RawCDR { get; }

        dynamic MappedCDR { get; }

        Decimal Duration { get; }

        DateTime EventTime { get; }

        List<VoiceEventPricedPart> PricedPartInfos { set; }
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
