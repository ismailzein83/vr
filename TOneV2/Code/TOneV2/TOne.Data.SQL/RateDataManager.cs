using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Data.SQL
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
