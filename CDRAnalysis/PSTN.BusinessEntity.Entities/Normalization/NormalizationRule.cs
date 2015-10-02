using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRule : Vanrise.Rules.Entities.BaseRule
    {
        public int NormalizationRuleId { get; set; }

        public NormalizationRuleCriteria Criteria { get; set; }

        public NormalizationRuleSettings Settings { get; set; }


        public override bool EvaluateAdvancedConditions(object target)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;

            if (this.Criteria.PhoneNumberPrefix != null && !cdr.PhoneNumber.StartsWith(this.Criteria.PhoneNumberPrefix))
                return false;

            if (this.Criteria.PhoneNumberType.HasValue && cdr.PhoneNumberType != this.Criteria.PhoneNumberType.Value)
                return false;

            return true; 
        }
    }
}
