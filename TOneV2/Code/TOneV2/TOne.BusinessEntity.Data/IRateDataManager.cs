using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IRateDataManager : IDataManager
    {
        void LoadCalculatedZoneRates(DateTime effectiveTime, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable);

        List<Rate> GetRate(int zoneId, string customerId, DateTime when);
    }
}
