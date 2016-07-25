using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Entities
{
    public class PartialMatchCDRBigResult : BigResult<PartialMatchCDR>
    {
        public PartialMatchCDR Summary { get; set; }
    }
}
