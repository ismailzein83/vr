using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.DurationVolumes
{
    public class VolumePerUnit : DurationVolumeSettings
    {
        public int NumberOfUnits { get; set; }

        public int UnitDurationInSec { get; set; }

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
