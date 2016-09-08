using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestCallTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public List<int?> SuppliersIds { get; set; }
        public List<string> SuppliersSourceIds { get; set; }
        public List<int?> CountryIds { get; set; }
        public List<long?> ZoneIds { get; set; }
        public string ZoneSourceId { get; set; }
        public int ProfileID { get; set; }
        public string ListEmails { get; set; }
        public int Quantity { get; set; }
    }
}
