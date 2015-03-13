using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class PriorityRouteActionData
    {
        public List<PriorityOption> Options { get; set; }
    }

    public class PriorityOption
    {
        public string SupplierId { get; set; }

        public int Priority { get; set; }

        public short? Percentage { get; set; }

        public bool Force { get; set; }
    }
}