using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public enum CDRType
    {
        Generated = 1, Recieved = 2
    }
    public class TCAnalMappedCDR
    {
        public long ID { get; set; }
        public Guid DataSourceId { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CalledNumber { get; set; }
        public string CallingNumber { get; set; }
        public CDRType CDRType { get; set; }
        public long OperatorID { get; set; }
        public string OrigCallingNumber { get; set; }
        public string OrigCalledNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsCorrelated { get; set; }
        public int? CallingNumberType { get; set; }
        public int CalledNumberType { get; set; }
    }
}
