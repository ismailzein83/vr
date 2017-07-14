using Retail.Cost.Entities;
using System;
using System.Collections.Generic;

namespace Retail.Cost.Data
{
    public interface ICDRCostDataManager : IDataManager
    {
        List<CDRCost> GetCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequests);

        void UpadeOverridenCostCDRAfterDate(DateTime? fromTime);

        HashSet<DateTime> GetDistinctDatesAfterId(long? cdrCostId);

        long? GetMaxCDRCostId();
    }
}