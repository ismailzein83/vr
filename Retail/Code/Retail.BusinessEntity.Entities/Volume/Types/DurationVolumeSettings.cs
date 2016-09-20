using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class DurationVolumeSettings : VolumeSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract DurationVolumeBalance CreateBalance(IDurationVolumeCreateBalanceContext context);

        public abstract void UpdateVolume(IDurationVolumeUpdateBalanceContext context);
    }

    public abstract class DurationVolumeBalance
    {

    }

    public interface IDurationVolumeCreateBalanceContext
    {

    }

    public interface IDurationVolumeUpdateBalanceContext
    {
        DurationVolumeBalance Balance { get; }

        Decimal CallDuration { get; }
    }
}
