using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum Duration {  Min = 0, Sec = 1 }
    public class RawCDRInput
    {
        public List<string> Switches { get; set; }
        public string InCarrier { get; set; }
        public string OutCarrier { get; set; }
        public string InCDPN { get; set; }
        public string OutCDPN { get; set; }
        public string CGPN { get; set; }
        public Duration DurationType { get; set; }
        public string MinDuration { get; set; }
        public string MaxDuration { get; set; }
        public int NRecords { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string WhereCondition { get; set; }
    }
}
