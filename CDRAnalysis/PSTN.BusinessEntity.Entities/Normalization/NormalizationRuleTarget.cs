﻿
namespace PSTN.BusinessEntity.Entities
{
    public abstract class NormalizationRuleTarget : Vanrise.Rules.BaseRuleTarget, IRulePhoneNumberTypeTarget, IRuleSwitchTarget, IRuleTrunkTarget, IRulePhoneNumberTarget
    {
        public abstract NormalizationRuleType RuleType { get; }

        public int? SwitchId { get; set; }

        public int? TrunkId { get; set; }

        public string PhoneNumber { get; set; }

        public NormalizationPhoneNumberType PhoneNumberType { get; set; }

        NormalizationPhoneNumberType? IRulePhoneNumberTypeTarget.PhoneNumberType
        {
            get { return this.PhoneNumberType; }
        }
    }
}
