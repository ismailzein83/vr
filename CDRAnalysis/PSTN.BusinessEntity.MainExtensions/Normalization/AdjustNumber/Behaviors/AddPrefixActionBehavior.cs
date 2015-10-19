using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.RuleTypes.NormalizeNumber.Actions
{
    public class AddPrefixActionBehavior : NormalizationRuleAdjustNumberActionBehavior
    {
        public override void Execute(NormalizationRuleAdjustNumberActionSettings settings, NormalizationRuleAdjustNumberTarget target)
        {
            AddPrefixActionSettings addPrefixActionSettings = settings as AddPrefixActionSettings;
            if (addPrefixActionSettings == null)
                throw new Exception(String.Format("{0} is not of type PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions.AddPrefixActionSettings", settings));

            target.PhoneNumber = String.Format("{0}{1}", addPrefixActionSettings.Prefix, target.PhoneNumber);
        }
    }
}
