using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Data;
using TOne.BI.Entities;

namespace TOne.BI.Business
{
    public class DestinationManager
    {
        public IEnumerable<ZoneValue> GetTopZonesByDuration(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, int topCount)
        {
            IDestinationDataManager dataManager = BIDataManagerFactory.GetDataManager<IDestinationDataManager>();
            return dataManager.GetTopZonesByDuration(timeDimensionType, fromDate, toDate, topCount);
        }
    }
}
