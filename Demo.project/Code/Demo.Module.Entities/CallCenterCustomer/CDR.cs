using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CDR
    {
        public long CDRId { get; set; }
        public DateTime Time { get; set; }
        public string Direction { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Duration { get; set; }
        public string Type { get; set; }
    }
}
