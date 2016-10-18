using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class SwapDealAnalysisOutboundRateCalcMethod
    {
        public abstract Guid ConfigId { get; }

        public abstract string ItemEditor { get; }
        
        public Guid CalculationMethodId { get; set; }
    }
}
