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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticByInterval), "StatisticItemId", "FirstCDRAttempt", "LastCDRAttempt", "CustomerId", "SupplierId", "Attempts", "TotalDurationInSeconds", "SaleZoneId", "SupplierZoneId", "PDDInSeconds", "MaxDurationInSeconds", "NumberOfCalls", "PortOut", "PortIn", "DeliveredAttempts", "SuccessfulAttempts");
        }
    }
}
