using System;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleQuery
    {
        public NormalizationPhoneNumberType? PhoneNumberType { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public string phoneNumberPrefix { get; set; }

        public int? PhoneNumberLength { get; set; }

        public List<int> SwitchIds { get; set; }

        public List<int> TrunkIds { get; set; }

        public string Description { get; set; }
    }
}
