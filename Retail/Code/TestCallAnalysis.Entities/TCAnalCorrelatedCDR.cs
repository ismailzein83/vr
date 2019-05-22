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
        public string GeneratedCalledNumber { get; set; }
        public string ReceivedCalledNumber { get; set; }
        public string GeneratedCallingNumber { get; set; }
        public string ReceivedCallingNumber { get; set; }
        public long? CalledOperatorID { get; set; }
        public long? CallingOperatorID { get; set; }
        public string OrigGeneratedCallingNumber { get; set; }
        public string OrigGeneratedCalledNumber { get; set; }
        public string OrigReceivedCallingNumber { get; set; }
        public string OrigReceivedCalledNumber { get; set; }
        public ReceivedCallingNumberType? ReceivedCallingNumberType { get; set; }
        public long? CaseId { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? GeneratedId { get; set; }
        public long ReceivedId { get; set; }
        public long? ClientId { get; set; }
    }
}
