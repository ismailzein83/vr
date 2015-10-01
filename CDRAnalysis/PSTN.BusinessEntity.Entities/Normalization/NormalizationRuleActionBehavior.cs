using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleActionBehavior
    {
        public abstract void Execute(NormalizationRuleActionSettings actionSettings, ref string phoneNumber);
    }
}
