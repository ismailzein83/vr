using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class RateDataManager : BaseTOneDataManager, IRateDataManager
    {
        public byte[] GetRateLastTimestamp()
        {
            object lastTimestamp = ExecuteScalarSP("LCR.sp_Rate_GetLastTimeStamp");
            return lastTimestamp != null ? (byte[])lastTimestamp : new byte[0];
        }
    }
}
