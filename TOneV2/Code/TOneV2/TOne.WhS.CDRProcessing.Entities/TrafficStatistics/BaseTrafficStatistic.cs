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
        public int SwitchID { get; set; }
        public int SupplierId { get; set; }

        public int Attempts { get; set; }
        public long SaleZoneId { get; set; }
        public long SupplierZoneId   { get; set; }
        public decimal PDDInSeconds { get; set; }
        public decimal MaxDurationInSeconds { get; set; }
        public int NumberOfCalls { get; set; }
        public string PortOut { get; set; }
        public string PortIn { get; set; }
        public int DeliveredAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public decimal DurationInSeconds { get; set; }

        public Decimal Utilization { get; set; }
        public int DeliveredNumberOfCalls { get; set; }
        public decimal PGAD { get; set; }
        public long CeiledDuration { get; set; }

    }
}
