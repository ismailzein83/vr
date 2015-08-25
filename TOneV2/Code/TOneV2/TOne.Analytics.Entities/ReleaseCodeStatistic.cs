using System;

namespace TOne.Analytics.Entities
{
    public class ReleaseCodeStatistic
    {
        public String ReleaseCode { get; set; }

        public String ReleaseSource { get; set; }

        public Decimal DurationsInMinutes { get; set; }

        public int Attempts { get; set; }

        public int FailedAttempts { get; set; }

        public DateTime FirstAttempt { get; set; }

        public DateTime LastAttempt { get; set; }

        public String PortOut { get; set; }

        public String PortIn { get; set; }

    }
}
