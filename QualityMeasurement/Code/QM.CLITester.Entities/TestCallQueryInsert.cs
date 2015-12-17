using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallQueryInput
    {
        public List<int> SupplierID { get; set; }
        public int CountryID { get; set; }
        public int ZoneID { get; set; }
        public int ProfileID { get; set; }
    }
}
