using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Normalization.Actions
{
    public class AddPrefixActionBehavior : Entities.NormalizationRuleActionBehavior
    {
        public override void Execute(Entities.NormalizationRuleActionSettings actionSettings, ref string phoneNumber)
        {
            Entities.Normalization.Actions.AddPrefixActionSettings addPrefixActionSettings = actionSettings as Entities.Normalization.Actions.AddPrefixActionSettings;

            if (addPrefixActionSettings == null)
                throw new Exception(String.Format("{0} is not of type Entities.Normalization.Actions.AddPrefixActionSettings", actionSettings));

            phoneNumber = String.Format("{0}{1}", addPrefixActionSettings.Prefix, phoneNumber);
        }
    }
}
