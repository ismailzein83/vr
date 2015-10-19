using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.RuleTypes.NormalizeNumber.Actions
{
    public class ReplaceStringActionBehavior : NormalizationRuleAdjustNumberActionBehavior
    {
        public override void Execute(NormalizationRuleAdjustNumberActionSettings settings, NormalizationRuleAdjustNumberTarget target)
        {
            ReplaceStringActionSettings replaceStringActionSettings = settings as ReplaceStringActionSettings;
            
            if (replaceStringActionSettings == null)
                throw new Exception(String.Format("{0} is not of type PSTN.BusinessEntity.Entities.Normalization.RuleTypes.NormalizeNumber.Actions.ReplaceStringActionSettings", settings));

            target.PhoneNumber = target.PhoneNumber.Replace(replaceStringActionSettings.StringToReplace, replaceStringActionSettings.NewString);
        }
    }
}
