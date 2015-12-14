using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallQueryInsert
    {
        public List<int> SupplierID { get; set; }
        public int CountryID { get; set; }
        public int ZoneID { get; set; }
        public int UserID { get; set; }
        public CallTestStatus CallTestStatus { get; set; }
        public CallTestResult CallTestResult { get; set; }
        public int InitiationRetryCount { get; set; }
        public int GetProgressRetryCount { get; set; }
    }
}
