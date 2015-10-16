using System;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleDetail
    {
        public NormalizationRule Entity { get; set; }

        public string SwitchNames { get; set; }

        public int SwitchCount { get; set; }

        public string TrunkNames { get; set; }

        public int TrunkCount { get; set; }

        public List<string> Descriptions { get; set; }
    }
}
