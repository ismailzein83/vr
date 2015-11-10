using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
    public interface ITrafficStatisticByIntervalDataManager : IDataManager
    {
        void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticByInterval> items);
        void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticByInterval> items);
    }
}
