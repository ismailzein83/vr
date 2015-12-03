using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
    public interface IBillingStatisticDataManager:IDataManager
    {
        void InsertBillingStatisticItemsToDB(IEnumerable<BillingStatistic> items);
        void UpdateBillingStatisticItemsInDB(IEnumerable<BillingStatistic> items);

        Dictionary<string, long> GetBillingStatisticItemsIdsByKeyFromDB(BillingStatisticBatch batch);
    }
}
