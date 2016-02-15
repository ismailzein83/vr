using PSTN.BusinessEntity.Entities;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class ReplaceStringActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
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
