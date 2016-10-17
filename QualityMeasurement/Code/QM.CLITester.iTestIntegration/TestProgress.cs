using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class TestProgress
    {
        public string Name { get; set; }
        public int TotalCalls { get; set; }
        public int CompletedCalls { get; set; }
        public int CliSuccess { get; set; }
        public int CliNoResult { get; set; }
        public int CliFail { get; set; }
        public string ShareUrl { get; set; }
        public string XmlResponse { get; set; }
        public string Result { get; set; }
        public List<CallResult> CallResults { get; set; }
    }

    public class CallResult
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Ring { get; set; }
        public string Call { get; set; }        
        public string ReleaseCode { get; set; }
        public string ReceivedCli { get; set; }
        public string Pdd { get; set; }
        public string Mos { get; set; }
        public DateTime? Duration { get; set; }
        public CallTestResult CallTestResult { get; set; }
        public string CallTestResultDescription { get; set; }
    }
}
