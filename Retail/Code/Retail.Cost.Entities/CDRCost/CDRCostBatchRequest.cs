using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Entities
{
    public class CDRCostBatchRequest
    {
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public List<string> CDPNs { get; set; }
    }
}
