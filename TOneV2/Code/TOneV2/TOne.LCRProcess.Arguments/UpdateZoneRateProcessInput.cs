using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCRProcess.Arguments
{
    public class UpdateZoneRateProcessInput
    {
        public bool IsFuture { get; set; }
        public DateTime RateEffectiveOn { get; set; }
    }
}
