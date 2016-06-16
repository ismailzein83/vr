using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.Entities
{
    public abstract class VoiceVolumeSettings : VolumeSettings
    {
        public int ConfigId { get; set; }

        public abstract VoiceVolumeBalance CreateBalance(IVoiceVolumeCreateBalanceContext context);

        public abstract void UpdateVolume(IVoiceVolumeUpdateBalanceContext context);
    }

    public abstract class VoiceVolumeBalance
    {

    }

    public interface IVoiceVolumeCreateBalanceContext
    {

    }

    public interface IVoiceVolumeUpdateBalanceContext
    {
        VoiceVolumeBalance Balance { get; }

        Decimal CallDuration { get; }
    }
}
