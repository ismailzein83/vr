using PSTN.BusinessEntity.Entities;
using System;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class AddPrefixActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("d235fd23-f660-496e-9f71-67226154727a"); } }

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
