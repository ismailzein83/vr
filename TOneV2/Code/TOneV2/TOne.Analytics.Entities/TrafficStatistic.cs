using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficStatistic
    {
        public DateTime FirstCDRAttempt { get; set; }

        public DateTime LastCDRAttempt { get; set; }

        public int Attempts { get; set; }

        public int DeliveredAttempts { get; set; }
        public int FailedAttempts { get; set; }
        
        public int SuccessfulAttempts { get; set; }

        public Decimal DurationsInMinutes { get; set; }
        public Decimal ACD { get; set; }
        
        public Decimal MaxDurationInMinutes { get; set; }
        public long CeiledDuration { get; set; }

        public Decimal PDDInSeconds { get; set; }

        public Decimal UtilizationInSeconds { get; set; }

        public int NumberOfCalls { get; set; }

        public int DeliveredNumberOfCalls { get; set; }

        public Decimal PGAD { get; set; }

        //public long CeiledDuration { get; set; }

        //public int ReleaseSourceAParty { get; set; }
    }
}
