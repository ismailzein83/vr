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
    public enum CDRNumberType
    {
        International = 1, Onnet = 2, Offnet=3
    }
    public class TCAnalMappedCDR
    {
        public long MappedCDRId { get; set; }
        public Guid DataSourceId { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CalledNumber { get; set; }
        public string CallingNumber { get; set; }
        public CDRType CDRType { get; set; }
        public long OperatorID { get; set; }
        public string OrigCallingNumber { get; set; }
        public string OrigCalledNumber { get; set; }
        public bool IsCorrelated { get; set; }
        public CDRNumberType? CallingNumberType { get; set; }
        public CDRNumberType? CalledNumberType { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
