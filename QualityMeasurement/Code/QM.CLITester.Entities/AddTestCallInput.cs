using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class AddTestCallInput
    {
        public List<int?> SuppliersIds { get; set; }
        public List<int> SuppliersSourceIds { get; set; }
        public int? ZoneID { get; set; }
        public string ZoneSourceId { get; set; }
        public int ProfileID { get; set; }
        //public string ListEmails { get; set; }
    }
}
