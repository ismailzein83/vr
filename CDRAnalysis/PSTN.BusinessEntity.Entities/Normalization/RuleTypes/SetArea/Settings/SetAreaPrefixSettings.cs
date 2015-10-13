using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleTypes.SetArea.Settings
{
    public class SetAreaPrefixSettings : NormalizationRuleSetAreaActionSettings
    {
        public int PrefixLength { get; set; }
    }
}
