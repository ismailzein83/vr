using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Voice.MainExtensions.VoiceVolumes
{
    public class VolumePerUnit : VoiceVolumeSettings
    {
        public int NumberOfUnits { get; set; }

        public int UnitDurationInSec { get; set; }

        public override VoiceVolumeBalance CreateBalance(IVoiceVolumeCreateBalanceContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVolume(IVoiceVolumeUpdateBalanceContext context)
        {
            throw new NotImplementedException();
        }
    }
}
