using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public abstract class BaseTrafficStatistic : Vanrise.Entities.StatisticManagement.IStatisticItem
    {
        public long StatisticItemId { get; set; }

        public DateTime FirstCDRAttempt { get; set; }

        public DateTime LastCDRAttempt { get; set; }

        public int CustomerId { get; set; }

        public int SupplierId { get; set; }

        public int Attempts { get; set; }
        public long SaleZoneId { get; set; }
        public long SupplierZoneId   { get; set; }
        public int PDDInSeconds { get; set; }
        public int MaxDurationInSeconds { get; set; }
        public int NumberOfCalls { get; set; }
        public string PortOut { get; set; }
        public string PortIn { get; set; }
        public int DeliveredAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public long TotalDurationInSeconds { get; set; }

        public static string GetStatisticItemKey(int customerId, int supplierId, long saleZoneId, long supplierZoneId, string portOut, string portIn)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", customerId, supplierId, saleZoneId, supplierZoneId, portOut, portIn);
        }
    }
}
