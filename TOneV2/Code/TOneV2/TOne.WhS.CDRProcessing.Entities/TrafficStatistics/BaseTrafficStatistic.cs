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
        public int PDDInSeconds { get; set; }
        public int MaxDurationInSeconds { get; set; }
        public int NumberOfCalls { get; set; }
        public string PortOut { get; set; }
        public string PortIn { get; set; }
        public int DeliveredAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public long DurationInSeconds { get; set; }

        public Decimal Utilization { get; set; }
        public int DeliveredNumberOfCalls { get; set; }
        public int PGAD { get; set; }
        public int CeiledDuration { get; set; }

    }
}
