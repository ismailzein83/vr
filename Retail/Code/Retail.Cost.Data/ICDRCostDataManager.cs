using Retail.Cost.Entities;
using System;
using System.Collections.Generic;

namespace Retail.Cost.Data
{
    public interface ICDRCostDataManager : IDataManager
    {
        List<CDRCost> GetCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequests);

        void UpadeOverridenCostCDRAfterId(long? cdrCostId);

        HashSet<DateTime> GetDistinctDatesAfterId(long? cdrCostId);

        long? GetMaxCDRCostId();
    }
}