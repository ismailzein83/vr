using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    
    public interface IRateDataManager : IDataManager
    {
        void LoadZoneRates(DateTime effectiveDate, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable);
    }
}