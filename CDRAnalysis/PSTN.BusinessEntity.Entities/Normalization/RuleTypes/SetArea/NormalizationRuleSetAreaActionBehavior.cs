using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleSetAreaActionBehavior
    {
        public abstract void Execute(NormalizationRuleSetAreaActionSettings settings, NormalizationRuleSetAreaTarget target);
    }
}
