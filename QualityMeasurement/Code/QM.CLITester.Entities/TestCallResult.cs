using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Calls_Total { get; set; }
        public string Calls_Complete { get; set; }
        public string CLI_Success { get; set; }
        public string CLI_No_Result { get; set; }
        public string CLI_Fail { get; set; }
        public string PDD { get; set; }
        public string Share_URL { get; set; }
    }
}
