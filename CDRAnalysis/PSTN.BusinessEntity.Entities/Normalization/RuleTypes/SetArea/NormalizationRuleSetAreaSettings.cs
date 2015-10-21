using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleSetAreaSettings : NormalizationRuleSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(INormalizationRuleSetAreaContext context, NormalizationRuleSetAreaTarget target);
    }
}
