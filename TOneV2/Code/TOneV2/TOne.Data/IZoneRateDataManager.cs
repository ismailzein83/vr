using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data
{
    public interface IZoneRateDataManager
    {
        void UpdateFromNewRates(byte[] ratesUpdatedAfter);
        void UpdateFromChangedRates(byte[] ratesUpdatedAfter);
        void ApplyEffectiveRates();
    }
}
