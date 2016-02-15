using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberSettings : NormalizationRuleSettings
    {
        public List<NormalizationRuleAdjustNumberActionSettings> Actions { get; set; }

        public override List<string> GetDescriptions()
        {
            if (this.Actions == null) return null;

            List<string> descriptions = new List<string>();

            foreach (NormalizationRuleAdjustNumberActionSettings action in this.Actions)
            {
                descriptions.Add(action.GetDescription());
            }

            return descriptions;
        }
    }
}
