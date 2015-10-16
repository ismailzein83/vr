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

        public IEnumerable<int> SwitchIds
        {
            get { return this.Criteria.SwitchIds; }
        }

        public IEnumerable<int> TrunkIds
        {
            get { return this.Criteria.TrunkIds; }
        }

        public IEnumerable<string> PhoneNumberPrefixes
        {
            get { return this.Criteria.PhoneNumberPrefix != null ? new List<string> { this.Criteria.PhoneNumberPrefix } : null; }
        }

        public IEnumerable<int> PhoneNumberLengths
        {
            get { return this.Criteria.PhoneNumberLength.HasValue ? new List<int> { this.Criteria.PhoneNumberLength.Value } : null; }
        }
    }
}
