using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleAdjustNumberActionBehavior
    {
        public abstract void Execute(NormalizationRuleAdjustNumberActionSettings settings, NormalizationRuleAdjustNumberTarget target);
    }
}
