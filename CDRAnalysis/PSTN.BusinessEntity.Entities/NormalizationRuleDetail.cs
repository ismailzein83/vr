using System;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleDetail
    {
        public int NormalizationRuleId { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }
}
