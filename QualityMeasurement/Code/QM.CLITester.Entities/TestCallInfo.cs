using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallInfo
    {
        public string SupplierName { get; set; }
        public string UserName { get; set; }
        public string CountryName { get; set; }
        public string ZoneName { get; set; }
        public string CallTestStatusDescription { get; set; }
        public string CallTestResultDescription { get; set; }
        public string ScheduleName { get; set; }
        public string Pdd { get; set; }
        public string Mos { get; set; }
        public DateTime CreationDate { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string ReceivedCli { get; set; }
        public string RingDuration { get; set; }
        public string CallDuration { get; set; }
        public string ReleaseCode { get; set; }
        public string Start { get; set; }
        public string ToMail { get; set; }
    }
}
