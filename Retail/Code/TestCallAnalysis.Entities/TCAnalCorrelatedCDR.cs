using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public enum ReceivedCallingNumberType { International = 1, Onnet = 2, Offnet = 3 }
    public enum GeneratedCallingNumberType { International = 1, Local = 2 }
    public class TCAnalCorrelatedCDR
    {
        public long CorrelatedCDRId { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CalledNumber { get; set; }
        public string GeneratedCallingNumber { get; set; }
        public string ReceivedCallingNumber { get; set; }
        public long OperatorID { get; set; }
        public string OrigCallingNumber { get; set; }
        public string OrigCalledNumber { get; set; }
        public ReceivedCallingNumberType? ReceivedCallingNumberType { get; set; }
        public long? ReceivedCallingNumberOperatorID { get; set; }
        public long? CaseId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
    }
}
