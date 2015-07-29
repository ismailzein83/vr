using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Entities
{
    public class TrafficStatisticBatch : PersistentQueueItem
    {
        static TrafficStatisticBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticBatch), "TrafficStatistics", "BatchStart", "BatchEnd");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatistic), "SwitchId", "Port_IN", "Port_OUT", "CustomerId", "OurZoneId", "OriginatingZoneId",
                "SupplierId", "SupplierZoneId", "FirstCDRAttempt", "LastCDRAttempt", "ID", "Attempts", "DeliveredAttempts",
                "SuccessfulAttempts", "DurationsInSeconds", "PDD", "PDDInSeconds", "MaxDurationInSeconds", "Utilization", "UtilizationInSeconds",
                "NumberOfCalls", "DeliveredNumberOfCalls", "PGAD", "CeiledDuration", "ReleaseSourceAParty");
        }

        public TrafficStatisticsByKey TrafficStatistics { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Traffic Statistics", TrafficStatistics.Count);
        }
    }
}