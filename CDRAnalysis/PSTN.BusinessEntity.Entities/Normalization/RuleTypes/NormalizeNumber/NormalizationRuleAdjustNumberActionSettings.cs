using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleAdjustNumberActionSettings
    {
        public int ConfigId { get; set; }

        public abstract string GetDescription();

        public abstract void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target);
    }
}
