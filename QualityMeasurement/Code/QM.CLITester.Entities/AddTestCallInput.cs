using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    ///// <summary>
    ///// Used in TOne V1 in "TOne.ClearVoiceIntegration" => Class AddTestCallInput
    ///// </summary>
    public class AddTestCallInput
    {
        public List<int?> SuppliersIds { get; set; }
        public List<string> SuppliersSourceIds { get; set; }
        public List<int?> CountryIds { get; set; }
        public List<long?> ZoneIds { get; set; }
        public string ZoneSourceId { get; set; }
        public int ProfileID { get; set; }
        public int UserId { get; set; }
        public int? ScheduleId { get; set; }
        public int Quantity { get; set; }
    }
}
