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
        void GetCalculatedZoneRates(DateTime effectiveTime, bool isFuture, IEnumerable<int> zoneIds, out List<ZoneRate> customerZoneRates, out List<ZoneRate> supplierZoneRates);
        List<Rate> GetRate(int zoneId, string customerId, DateTime when);

        List<ExchangeRate> GetExchangeRates(DateTime fromDate, DateTime toDate);
    }
}
