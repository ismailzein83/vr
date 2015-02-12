using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCRProcess.Arguments
{
   public class BuildRouteProcessInput
    {
        public bool IsFuture { get; set; }
        public DateTime CodeEffectiveOn { get; set; }

        public int DistinctCodesBatchSize { get; set; }
    }
}
