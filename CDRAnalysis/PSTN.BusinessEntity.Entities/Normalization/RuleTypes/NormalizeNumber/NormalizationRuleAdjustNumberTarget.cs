using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberTarget : NormalizationRuleTarget
    {        
        public override NormalizationRuleType RuleType
        {
            get { return NormalizationRuleType.AdjustNumber; }
        }
    }
}
