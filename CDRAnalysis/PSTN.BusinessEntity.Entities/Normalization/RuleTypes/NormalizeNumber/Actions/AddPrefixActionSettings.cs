using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions
{
    public class AddPrefixActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
        public string Prefix { get; set; }

        public override string GetDescription()
        {
            return string.Format("Add Prefix: Prefix = {0}", Prefix);
        }
    }
}
