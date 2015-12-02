using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class TestCallResult
    {
        public int Id { get; set; }
        public int SupplierID { get; set; }
        public int CountryID { get; set; }
        public int ZoneID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Test_ID { get; set; }
        public string Name { get; set; }
        public int Calls_Total { get; set; }
        public int Calls_Complete { get; set; }
        public int CLI_Success { get; set; }
        public int CLI_No_Result { get; set; }
        public int CLI_Fail { get; set; }
        public int PDD { get; set; }
        public string Share_URL { get; set; }
        public int Status { get; set; }
    }
}
