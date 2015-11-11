using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
    public interface ITrafficStatisticDailyDataManager:IDataManager
    {
        void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticDaily> items);
        void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticDaily> items);

        Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticDailyBatch  batch);
    }
}
