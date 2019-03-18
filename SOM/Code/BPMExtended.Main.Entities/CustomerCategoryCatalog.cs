using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerCategoryCatalog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SegmentId { get; set; }
        public string IsNormal { get; set; }
        public string IsSkipPayment { get; set; }
        public string CustomerCategory { get; set; }
    }
}
