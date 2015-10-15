using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleSetAreaBehavior
    {
        public abstract void Execute(NormalizationRuleSetAreaSettings settings, NormalizationRuleSetAreaTarget target);
    }
}
