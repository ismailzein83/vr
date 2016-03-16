using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;

namespace InterConnect.BusinessEntity.Entities
{
    public class TrafficStats : ISummaryItem
    {
        static TrafficStats()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStats), "BatchStart", "CDPN", "OperatorId", "TotalDuration", "Attempts", "SummaryItemId");
        }
        public DateTime BatchStart { get; set; }

        public string CDPN { get; set; }

        public int OperatorId { get; set; }

        public Decimal TotalDuration { get; set; }

        public int Attempts { get; set; }

        public long SummaryItemId
        {
            get;
            set;
        }
    }

    public class TrafficStatsBatch : Vanrise.Queueing.Entities.PersistentQueueItem, ISummaryBatch<TrafficStats>
    {
        static TrafficStatsBatch()
        {
            TrafficStats dummy = new TrafficStats();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatsBatch), "BatchStart", "Items");
        }

        public IEnumerable<TrafficStats> Items { get; set; }

        public DateTime BatchStart { get; set; }

        public override string GenerateDescription()
        {
            return string.Format("{0} Traffic Stats", this.Items.Count());
        }
    }

    public class TrafficStatsBatchStageItemType : Vanrise.Queueing.Entities.QueueExecutionFlowStageItemType
    {
        public override Type GetQueueItemType()
        {
            return typeof(TrafficStatsBatch);
        }
    }

}
