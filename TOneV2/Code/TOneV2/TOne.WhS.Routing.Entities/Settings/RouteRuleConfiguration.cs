using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleConfiguration
    {
        public FixedOptionLossType FixedOptionLossType { get; set; }

        public bool FixedOptionLossDefaultValue { get; set; } 
    }

    public enum FixedOptionLossType
    {
        RemoveLoss = 0,
        AcceptLoss = 1
    }
}
