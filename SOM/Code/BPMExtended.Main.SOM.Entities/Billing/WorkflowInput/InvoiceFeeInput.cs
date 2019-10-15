using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class InvoiceFeeInput
    {
        public string RatePlanId { get; set; }
        public List<string> ServiceIds { get; set; }
    }
}
