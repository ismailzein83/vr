using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.MainExtensions.Normalization.AdjustNumber
{
    public class SubstringActionSettings : NormalizationRuleAdjustNumberActionSettings
    {
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
