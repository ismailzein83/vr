using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.RuleTypes.NormalizeNumber.Actions
{
    public class SubstringActionBehavior : NormalizationRuleAdjustNumberActionBehavior
    {
        public override void Execute(NormalizationRuleAdjustNumberActionSettings settings, NormalizationRuleAdjustNumberTarget target)
        {
            SubstringActionSettings substringActionSettings = settings as SubstringActionSettings;
            if (substringActionSettings == null)
                throw new Exception(String.Format("{0} is not of type PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions.SubstringActionSettings", settings));

            if (target.PhoneNumber.Length > substringActionSettings.StartIndex)
                target.PhoneNumber = target.PhoneNumber.Substring(substringActionSettings.StartIndex, Math.Min(target.PhoneNumber.Length - substringActionSettings.StartIndex, substringActionSettings.Length));
        }
    }
}
