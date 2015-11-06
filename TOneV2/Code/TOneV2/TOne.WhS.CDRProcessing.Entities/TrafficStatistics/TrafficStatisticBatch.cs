using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticBatch<T> : Vanrise.Entities.StatisticManagement.IStatisticBatch<T>
        where T : BaseTrafficStatistic
    {
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public Dictionary<string, T> ItemsByKey { get; set; }
    }
}
