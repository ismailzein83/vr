using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.SetArea
{
    public class SetAreaPrefixSettings : NormalizationRuleSetAreaSettings
    {
        public override Guid ConfigId { get { return new Guid("b1281b56-e156-40a2-abcf-3766a80c63a0"); } }

        public int PrefixLength { get; set; }

        public override List<string> GetDescriptions()
        {
            List<string> descriptions = new List<string>();

            descriptions.Add("Set Area Prefix: Prefix Length = " + this.PrefixLength);

            return descriptions;
        }

        public override void Execute(INormalizationRuleSetAreaContext context, NormalizationRuleSetAreaTarget target)
        {
            target.AreaCode = target.PhoneNumber.Substring(0, Math.Min(this.PrefixLength, target.PhoneNumber.Length));
        }
    }
}
