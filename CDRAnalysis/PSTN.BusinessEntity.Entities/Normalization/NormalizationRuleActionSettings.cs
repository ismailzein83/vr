using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleActionSettings
    {
        public int BehaviorId { get; set; }

        public abstract string GetDescription();
    }
}
