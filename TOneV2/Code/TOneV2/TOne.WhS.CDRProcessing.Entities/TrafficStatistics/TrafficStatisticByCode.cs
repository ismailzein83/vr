using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticByCode : BaseTrafficStatistic, Vanrise.Entities.StatisticManagement.IRawItem
    {
        static TrafficStatisticByCode()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TrafficStatisticByCode), "StatisticItemId", "SwitchID", "FirstCDRAttempt", "LastCDRAttempt", "CustomerId", "SupplierId", "Attempts", "DurationInSeconds", "SaleZoneId", "SupplierZoneId", "PDDInSeconds", "MaxDurationInSeconds", "NumberOfCalls", "PortOut", "PortIn", "DeliveredAttempts", "SuccessfulAttempts", "Utilization", "DeliveredNumberOfCalls", "PGAD", "CeiledDuration", "SaleCode", "SupplierCode");
        }
        public string SaleCode { get; set; }
        public string SupplierCode { get; set; }
        public static string GetStatisticItemKey(int customerId, int supplierId, long saleZoneId, long supplierZoneId, string portOut, string portIn, int switchID, string saleCode, string supplierCode)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}", customerId, supplierId, saleZoneId, supplierZoneId, portOut, portIn, switchID, saleCode, supplierCode);
        }
    }
}
