﻿using System;
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
}
