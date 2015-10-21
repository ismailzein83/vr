using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class AddPrefixActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
        public string Prefix { get; set; }

        public override string GetDescription()
        {
            return string.Format("Add Prefix: Prefix = {0}", Prefix);
        }

        public override void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target)
        {
            target.PhoneNumber = String.Format("{0}{1}", this.Prefix, target.PhoneNumber);
        }
    }
}
