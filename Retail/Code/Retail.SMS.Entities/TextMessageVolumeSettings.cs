using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.SMS.Entities
{
    public abstract class TextMessageVolumeSettings : VolumeSettings
    {
        public Guid ConfigId { get; set; }

        public abstract TextMessageVolumeBalance CreateBalance(ITextMessageVolumeCreateBalanceContext context);

        public abstract void UpdateVolume(ITextMessageVolumeUpdateBalanceContext context);
    }

    public abstract class TextMessageVolumeBalance
    {

    }

    public interface ITextMessageVolumeCreateBalanceContext
    {

    }

    public interface ITextMessageVolumeUpdateBalanceContext
    {
        TextMessageVolumeBalance Balance { get; }

        int MessageNbOfCharacters { get; }
    }
}
