using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ServiceHistoryRecord
    {
        public string ServiceId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ValidFromDate { get; set; }
    }
}
