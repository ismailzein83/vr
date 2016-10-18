using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class SwapDealAnalysisOutboundRateFixed : SwapDealAnalysisOutboundRateCalcMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("EA1454A9-0FA0-4B16-93E8-76533BD504F3"); }
        }

        public override string ItemEditor
        {
            get { return "fill here the url to item editor"; }
        }
    }
}
