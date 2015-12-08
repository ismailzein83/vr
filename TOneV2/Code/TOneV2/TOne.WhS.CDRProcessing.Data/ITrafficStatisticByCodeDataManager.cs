using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
    public interface ITrafficStatisticByCodeDataManager : IDataManager
    {
        void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticByCode> items);
        void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticByCode> items);

        Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticByCodeBatch batch);
    }
}
