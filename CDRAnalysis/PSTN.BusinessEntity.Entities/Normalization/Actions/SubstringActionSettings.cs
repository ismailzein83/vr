using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.Actions
{
    public class SubstringActionSettings : NormalizationRuleActionSettings
    {
        public int StartIndex { get; set; }

        public int Length { get; set; }

        public override string GetDescription()
        {
            return string.Format("Substring: Start Index = {0}, Length = {1}", StartIndex, Length);
        }
    }
}
