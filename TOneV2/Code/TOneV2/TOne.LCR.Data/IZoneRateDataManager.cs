using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCR.Data
{
    public interface IZoneRateDataManager : IDataManager
    {
        void UpdateFromNewRates(byte[] ratesUpdatedAfter);
        void UpdateFromChangedRates(byte[] ratesUpdatedAfter);
        void ApplyEffectiveRates();
    }
}
