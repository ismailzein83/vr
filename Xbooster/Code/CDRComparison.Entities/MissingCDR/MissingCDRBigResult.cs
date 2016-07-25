using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Entities
{
    public class MissingCDRBigResult : BigResult<MissingCDR>
    {
        public MissingCDR Summary { get; set; }
    }
}
