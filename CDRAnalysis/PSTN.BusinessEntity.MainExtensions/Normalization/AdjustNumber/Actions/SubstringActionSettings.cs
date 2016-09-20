using PSTN.BusinessEntity.Entities;
using System;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class SubstringActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("819f222e-5a68-4563-9655-f4298c34453e"); } }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex, Length);
        }

        public override void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target)
        {
            if (target.PhoneNumber.Length > this.StartIndex)
                target.PhoneNumber = target.PhoneNumber.Substring(this.StartIndex, Math.Min(target.PhoneNumber.Length - this.StartIndex, this.Length));
        }
    }
}
