using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficStatisticGroupSummary
    {
        public object[] GroupKeyValues { get; set; }

        public DateTime FirstCDRAttempt { get; set; }

        public DateTime LastCDRAttempt { get; set; }

        public int Attempts { get; set; }

        public int DeliveredAttempts { get; set; }

        public int SuccessfulAttempts { get; set; }

        public Decimal DurationsInSeconds { get; set; }

        public Decimal MaxDurationInSeconds { get; set; }

        public Decimal PDDInSeconds { get; set; }

        public Decimal UtilizationInSeconds { get; set; }

        public int NumberOfCalls { get; set; }

        public int DeliveredNumberOfCalls { get; set; }

        public Decimal PGAD { get; set; }

        //public long CeiledDuration { get; set; }

        //public int ReleaseSourceAParty { get; set; }
    }
}