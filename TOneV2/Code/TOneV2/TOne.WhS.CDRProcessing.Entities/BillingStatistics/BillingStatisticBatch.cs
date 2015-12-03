using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingStatisticBatch : PersistentQueueItem, Vanrise.Entities.StatisticManagement.IStatisticBatch<BillingStatistic>
    {
        static BillingStatisticBatch()

       {
           BillingStatistic TrafficStatisticByInterval = new BillingStatistic();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingStatisticBatch), "ItemsByKey", "BatchStart", "BatchEnd");
       }
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public Dictionary<string, BillingStatistic> ItemsByKey { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("BillingStatsBatch of {0} BillingStats", ItemsByKey.Count());
        }
    }
}
