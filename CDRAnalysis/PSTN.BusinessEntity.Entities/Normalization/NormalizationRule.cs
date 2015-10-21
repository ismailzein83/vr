using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRule : Vanrise.Rules.BaseRule, IRulePhoneNumberTypeCriteria, IRuleSwitchCriteria, IRuleTrunkCriteria, IRulePhoneNumberPrefixCriteria, IRulePhoneNumberLengthCriteria
    {
        public NormalizationRuleCriteria Criteria { get; set; }

        public NormalizationRuleSettings Settings { get; set; }

        public string Description { get; set; }

        public IEnumerable<NormalizationPhoneNumberType> PhoneNumberTypes
        {
            get { return new List<NormalizationPhoneNumberType> { this.Criteria.PhoneNumberType }; }
        }

        IEnumerable<int> IRuleSwitchCriteria.SwitchIds
        {
            get { return this.Criteria.SwitchIds; }
        }

        IEnumerable<int> IRuleTrunkCriteria.TrunkIds
        {
            get { return this.Criteria.TrunkIds; }
        }

        IEnumerable<string> IRulePhoneNumberPrefixCriteria.PhoneNumberPrefixes
        {
            get { return this.Criteria.PhoneNumberPrefix != null ? new List<string> { this.Criteria.PhoneNumberPrefix } : null; }
        }

        IEnumerable<int> IRulePhoneNumberLengthCriteria.PhoneNumberLengths
        {
            get { return this.Criteria.PhoneNumberLength.HasValue ? new List<int> { this.Criteria.PhoneNumberLength.Value } : null; }
        }
    }
}
