using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.Actions
{
    public class SubstringActionBehavior : Entities.NormalizationRuleActionBehavior
    {
        public override void Execute(Entities.NormalizationRuleActionSettings actionSettings, ref string phoneNumber)
        {
            Entities.Normalization.Actions.SubstringActionSettings substringActionSettings = actionSettings as Entities.Normalization.Actions.SubstringActionSettings;

            if (substringActionSettings == null)
                throw new Exception(String.Format("{0} is not of type Entities.Normalization.Actions.SubstringActionSettings", actionSettings));

            if (phoneNumber.Length > substringActionSettings.StartIndex)
                phoneNumber = phoneNumber.Substring(substringActionSettings.StartIndex, Math.Min(phoneNumber.Length - substringActionSettings.StartIndex, substringActionSettings.Length));
        }
    }
}
