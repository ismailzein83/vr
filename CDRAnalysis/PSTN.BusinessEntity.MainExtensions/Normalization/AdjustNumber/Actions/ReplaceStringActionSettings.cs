using PSTN.BusinessEntity.Entities;
using System;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class ReplaceStringActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("a34217b8-79a0-4eae-bb96-59badb29dc03"); } }

        public string StringToReplace { get; set; }

        public string NewString { get; set; }

        public override string GetDescription()
        {
            return string.Format("Replace String: String To Replace = {0}, NewString = {1}", this.StringToReplace, this.NewString);
        }

        public override void Execute(INormalizationRuleAdjustNumberActionContext context, NormalizationRuleAdjustNumberTarget target)
        {
            target.PhoneNumber = target.PhoneNumber.Replace(this.StringToReplace, this.NewString);
        }
    }
}
