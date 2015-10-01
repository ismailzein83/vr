using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRule
    {
        public int NormalizationRuleId { get; set; }

        public NormalizationRuleCriteria Criteria { get; set; }

        public NormalizationRuleSettings Settings { get; set; }
    }
}
