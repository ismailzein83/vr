using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class NormalizationRule : Vanrise.Rules.BaseRule
    {
        public NormalizationRuleCriteria Criteria { get; set; }

        public NormalizationRuleSettings Settings { get; set; }

        public string Description { get; set; }
      
    }
}
