using System;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleQuery
    {
        public DateTime? BeginEffectiveDate { get; set; }
        
        public DateTime? EndEffectiveDate { get; set; }

        public List<int> SwitchIds { get; set; }

        public List<int> TrunkIds { get; set; }

        public NormalizationPhoneNumberType? PhoneNumberType { get; set; }

        public int? PhoneNumberLength { get; set; }

        public string phoneNumberPrefix { get; set; }
    }
}
