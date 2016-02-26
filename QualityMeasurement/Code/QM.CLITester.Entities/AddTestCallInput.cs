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
        public List<string> SuppliersSourceIds { get; set; }
        public int? CountryID { get; set; }
        public long? ZoneID { get; set; }
        public string ZoneSourceId { get; set; }
        public int ProfileID { get; set; }
        public int UserId { get; set; }
        public int? ScheduleId { get; set; }
    }
}
