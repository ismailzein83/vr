using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data.SQL
{
    public class ZoneRateDataManager : BaseTOneDataManager, IZoneRateDataManager
    {
        public void UpdateFromNewRates(byte[] ratesUpdatedAfter)
        {
            int rowsAffected = ExecuteNonQuery("LCR.sp_ZoneRate_UpdateFromNewRates", ratesUpdatedAfter);
        }

        public void UpdateFromChangedRates(byte[] ratesUpdatedAfter)
        {
            int rowsAffected = ExecuteNonQuery("LCR.sp_ZoneRate_UpdateFromChangedRates", ratesUpdatedAfter);
        }

        public void ApplyEffectiveRates()
        {
            ExecuteNonQuery("LCR.sp_ZoneRate_ApplyEffectiveRates");
        }
    }
}
