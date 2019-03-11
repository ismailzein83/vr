using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class TCAnalCaseCDR
    {
        public long CaseId { get; set; }
        public string CallingNumber {get; set;}
        public string CalledNumber {get; set;}
        public DateTime FirstAttempt {get; set;}
        public DateTime LastAttempt {get; set;}
        public int NumberOfCDRs {get; set;}
        public Guid StatusId {get; set;}
        public long OperatorID {get; set;}
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
    }
}
