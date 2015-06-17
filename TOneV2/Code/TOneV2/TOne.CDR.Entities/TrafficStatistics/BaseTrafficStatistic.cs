using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public abstract class BaseTrafficStatistic
    {
        public long ID { get; set; }

        public int Attempts { get; set; }

        public int DeliveredAttempts { get; set; }

        public int SuccessfulAttempts { get; set; }

        public decimal DurationsInSeconds { get; set; }

        public TimeSpan? PDD { get; set; }

        public decimal? PDDInSeconds { get; set; }

        public decimal MaxDurationInSeconds { get; set; }

        public TimeSpan Utilization { get; set; }

        public decimal UtilizationInSeconds { get; set; }

        public int NumberOfCalls { get; set; }

        public int DeliveredNumberOfCalls { get; set; }

        public decimal PGAD { get; set; }

        public int CeiledDuration { get; set; }

        public int ReleaseSourceAParty { get; set; }

        public int ReleaseSourceS { get; set; }

        public abstract string GetGroupKey();
    }
}
