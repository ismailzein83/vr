using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticByInterval : BaseTrafficStatistic, Vanrise.Entities.StatisticManagement.IRawItem
    {
        static TrafficStatisticByInterval()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticByInterval), "StatisticItemId","SwitchID", "FirstCDRAttempt", "LastCDRAttempt", "CustomerId", "SupplierId", "Attempts", "DurationInSeconds", "SaleZoneId", "SupplierZoneId", "PDDInSeconds", "MaxDurationInSeconds", "NumberOfCalls", "PortOut", "PortIn", "DeliveredAttempts", "SuccessfulAttempts", "Utilization", "DeliveredNumberOfCalls", "PGAD", "CeiledDuration");
        }
    }
}
