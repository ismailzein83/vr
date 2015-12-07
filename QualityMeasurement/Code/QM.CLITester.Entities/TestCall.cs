using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCall
    {
        public long ID { get; set; }
        public int SupplierID { get; set; }
        public int CountryID { get; set; }
        public int ZoneID { get; set; }
        public DateTime CreationDate { get; set; }
        public string InitiateTestInformation { get; set; }
        public string TestProgress { get; set; }
        public int? CallTestStatus { get; set; }
        public int? CallTestResult { get; set; }
    }
}
