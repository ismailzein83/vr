using System;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleDetail
    {
        public int NormalizationRuleId { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string SwitchNames { get; set; }

        public int SwitchCount { get; set; }

        public string TrunkNames { get; set; }

        public int TrunkCount { get; set; }

        public NormalizationPhoneNumberType PhoneNumberType { get; set; }

        public int? PhoneNumberLength { get; set; }

        public string PhoneNumberPrefix { get; set; }

        public List<string> Descriptions { get; set; }

        public string Description { get; set; }
    }
}
