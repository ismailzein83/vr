using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Data
{
    public interface ITrafficStatsDataManager : IDataManager
    {
        IEnumerable<TrafficStats> GetTrafficStats(DateTime batchStart);

        void Insert(List<TrafficStats> itemsToAdd);

        void Update(List<TrafficStats> itemsToUpdate);
    }
}
