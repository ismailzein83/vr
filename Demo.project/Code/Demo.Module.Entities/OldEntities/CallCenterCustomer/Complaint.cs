using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Complaint
    {
        public long ComplaintId { get; set; }
        public DateTime Time { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Agent { get; set; }
        public string Notes { get; set; }

    }
}
