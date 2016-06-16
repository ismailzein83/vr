using Retail.SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.SMS.MainExtensions.TextMessageVolumes
{
    public class TextMessageQuantityVolume : TextMessageVolumeSettings
    {
        public int NumberOfMessages { get; set; }

        public int? MaxNbOfCharactersPerMessage { get; set; }

        public override TextMessageVolumeBalance CreateBalance(ITextMessageVolumeCreateBalanceContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVolume(ITextMessageVolumeUpdateBalanceContext context)
        {
            throw new NotImplementedException();
        }
    }
}
