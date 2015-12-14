using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SwitchIdentificationRule : Vanrise.Rules.BaseRule,IRuleSwitchDataSourceCriteria
    {
        public string Description { get; set; }  
        public SwitchIdentificationRuleCriteria Criteria { get; set; }
        public SwitchIdentificationRuleSettings Settings { get; set; }

        List<int> IRuleSwitchDataSourceCriteria.DataSources
        {
            get { return this.Criteria.DataSources; }
        }

    }
}
