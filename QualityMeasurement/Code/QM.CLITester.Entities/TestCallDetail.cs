using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallDetail
    {
        public TestCall Entity { get; set; }
        public string SupplierName { get; set; }
        public string CountryName { get; set; }
        public string ZoneName { get; set; }
        public string CallTestStatusDescription { get; set; }
        public string CallTestResultDescription { get; set; }
    }
}
