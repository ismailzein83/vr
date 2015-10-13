using PSTN.BusinessEntity.Entities;
using PSTN.BusinessEntity.Entities.Normalization.RuleTypes.SetArea.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.RuleTypes.SetArea.Behaviors
{
    public class SetAreaPrefixBehavior : NormalizationRuleSetAreaActionBehavior
    {
        public override void Execute(NormalizationRuleSetAreaActionSettings settings, NormalizationRuleSetAreaTarget target)
        {
            SetAreaPrefixSettings setAreaPrefixSettings = settings as SetAreaPrefixSettings;
            if(setAreaPrefixSettings == null)
                throw new Exception(String.Format("{0} is not of type PSTN.BusinessEntity.Entities.Normalization.RuleTypes.SetArea.Settings.SetAreaPrefixSettings", settings));

            target.AreaCode = target.PhoneNumber.Substring(0, Math.Min(setAreaPrefixSettings.PrefixLength, target.PhoneNumber.Length));
        }
    }
}
