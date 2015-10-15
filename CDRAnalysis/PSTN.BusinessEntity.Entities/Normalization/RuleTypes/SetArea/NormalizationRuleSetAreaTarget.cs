using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleSetAreaTarget : NormalizationRuleTarget
    {
        public string AreaCode { get; set; }

        public override NormalizationRuleType RuleType
        {
            get { return NormalizationRuleType.SetArea; }
        }
    }
}
