
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.DurationVolumes
{
    public class VolumeByDuration : DurationVolumeSettings
    {
        public int DurationInSec { get; set; }

        public override DurationVolumeBalance CreateBalance(IDurationVolumeCreateBalanceContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVolume(IDurationVolumeUpdateBalanceContext context)
        {
            throw new NotImplementedException();
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
