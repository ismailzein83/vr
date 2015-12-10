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
        public int UserID { get; set; }
        public DateTime CreationDate { get; set; }
        public Object InitiateTestInformation { get; set; }
        public Object TestProgress { get; set; }
        public CallTestStatus CallTestStatus { get; set; }
        public CallTestResult CallTestResult { get; set; }
    }
}
