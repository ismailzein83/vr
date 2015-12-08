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
        public static string GetStatisticItemKey(int customerId, int supplierId, long saleZoneId, long supplierZoneId, string portOut, string portIn, int switchID)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", customerId, supplierId, saleZoneId, supplierZoneId, portOut, portIn, switchID);
        }
    }
}
