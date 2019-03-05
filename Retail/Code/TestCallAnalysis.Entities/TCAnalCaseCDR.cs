using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class TCAnalCaseCDR
    {
        public long ID { get; set; }
        public string CallingNumber {get; set;}
        public string CalledNumber {get; set;}
        public DateTime FirstAttempt {get; set;}
        public DateTime LastAttempt {get; set;}
        public int NumberOfCDRs {get; set;}
        public Guid StatusId {get; set;}
        public long OperatorID {get; set;}
    }
}
