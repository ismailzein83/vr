using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities.Settings
{
    public class SwapDealSettingData : SettingData
    {
        public Guid DefaultCalculationMethodId { get; set; }

        public Dictionary<Guid, SwapDealAnalysisOutboundRateCalcMethod> CalculationMethods { get; set; }
    }
}
