using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class ReleaseCodeStat
    {
        public int SwitchId { get; set; }
        public string ReleaseCode { get; set; }
        public string ReleaseSource { get; set; }

        public long MasterPlanZoneId { get; set; }
        public long SaleZoneId { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public int Attempt { get; set; }
        public DateTime? FirstAttempt { get; set; }
        public DateTime? LastAttempt { get; set; }
        public decimal Percentage { get; set; }
        public int FailedAttempt { get; set; }
        public decimal DurationInMinutes { get; set; }
        public string PortIn { get; set; }
        public string PortOut { get; set; }
        public string GateWayIn { get; set; }
        public string GateWayOut { get; set; }
    }
}
