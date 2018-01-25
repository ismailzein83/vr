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

        void DeductFromBalances(IVoiceUsageChargerDeductFromBalanceContext context);
    }

    public interface IPackageSettingVoiceUsageCharger
    {
        bool TryGetVoiceUsageCharger(Guid serviceTypeId, out IPackageVoiceUsageCharger voiceUsageCharging);
    }

    public interface IVoiceUsageChargerContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        long PackageAccountId { get; }

        Guid ServiceTypeId { get; }

        dynamic MappedCDR { get; }

        Decimal Duration { get; }

        DateTime EventTime { get; }

        List<VoiceEventPricedPart> PricedPartInfos { set; }

        Object ChargeInfo { set; }
    }

    public interface IVoiceUsageChargerDeductFromBalanceContext
    {
        long AccountId { get; }

        Guid ServiceTypeId { get; }

        Object ChargeInfo { get; }
    }
}
