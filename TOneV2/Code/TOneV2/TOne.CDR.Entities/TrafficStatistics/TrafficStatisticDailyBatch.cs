using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class TrafficStatisticDailyBatch : PersistentQueueItem
    {
        static TrafficStatisticDailyBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticDailyBatch), "TrafficStatistics", "BatchStart", "BatchEnd");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticDaily), "SwitchId", "CustomerId", "OurZoneId", "OriginatingZoneId", "SupplierId", "SupplierZoneId",
                "CallDate", "ID", "Attempts", "DeliveredAttempts", "SuccessfulAttempts", "DurationsInSeconds", "PDD",
                "PDDInSeconds", "MaxDurationInSeconds", "Utilization", "UtilizationInSeconds", "NumberOfCalls", "DeliveredNumberOfCalls", "PGAD",
                "CeiledDuration", "ReleaseSourceAParty");
        }

        public TrafficStatisticsDailyByKey TrafficStatistics { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Daily Traffic Statistics", TrafficStatistics.Count);
        }
    }
}
