using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class HourlyReport
    {
        public int Hour { get; set; }

        public DateTime Date { get; set; }

        public int Attempts { get; set; }
        public Decimal DurationsInMinutes { get; set; }
        public Decimal ASR { get; set; }
        public Decimal ACD { get; set; }
        public Decimal NER { get; set; }
        public Decimal DeliveredASR { get; set; }
        public int FailedAttempts { get; set; }
        public Decimal MaxDurationInMinutes { get; set; }

        public DateTime LastAttempt { get; set; }
        public int SuccessfulAttempt { get; set; }

        public Decimal UtilizationInMinutes { get; set; }
        public Decimal GrayArea { get; set; }
        public Decimal GreenArea { get; set; }
        
    }
}
