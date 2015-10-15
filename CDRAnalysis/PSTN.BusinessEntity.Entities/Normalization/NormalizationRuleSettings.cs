using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public enum NormalizationRuleType { AdjustNumber = 0, SetArea = 1 }

    public class NormalizationRuleSettings
    {
        public NormalizationRuleType RuleType { get; set; }

        public int ConfigId { get; set; }

        public List<NormalizationRuleActionSettings> Actions { get; set; }
    }
}
