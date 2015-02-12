using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TopNDestinationView
    {
        public int OurZoneID { get; set; }
        public string ZoneName { get; set; }
        public string SupplierID { get; set; }
        public int Attempts { get; set; }
        public decimal DurationInMinutes { get; set; }
        public decimal ASR { get; set; }
        public int SuccessfulAttempts { get; set; }
        public decimal ACD { get; set; }
        public decimal DeliveredASR { get; set; }
        public decimal AveragePDD { get; set; }
        public string CodeGroup { get; set; }

    }
}
