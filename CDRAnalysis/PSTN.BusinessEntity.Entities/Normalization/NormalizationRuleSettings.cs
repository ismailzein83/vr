using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public enum NormalizationRuleType
    {
        [Description("Adjust Number")]
        AdjustNumber = 0,
        [Description("Set Area")]
        SetArea = 1
    }

    public abstract class NormalizationRuleSettings
    {
        public NormalizationRuleType RuleType { get; set; }

        public abstract List<string> GetDescriptions();
    }
}
