using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticDaily : BaseTrafficStatistic
    {
        static TrafficStatisticDaily()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticDaily), "StatisticItemId","SwitchID", "FirstCDRAttempt", "LastCDRAttempt", "CustomerId", "SupplierId", "Attempts", "DurationInSeconds", "SaleZoneId", "SupplierZoneId", "PDDInSeconds", "MaxDurationInSeconds", "NumberOfCalls", "PortOut", "PortIn", "DeliveredAttempts", "SuccessfulAttempts", "Utilization", "DeliveredNumberOfCalls", "PGAD", "CeiledDuration");
        }
    }
}
