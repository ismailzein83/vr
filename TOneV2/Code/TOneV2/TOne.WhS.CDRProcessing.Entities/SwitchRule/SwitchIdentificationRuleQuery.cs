using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SwitchIdentificationRuleQuery
    {
        public string Description{ get; set; }
        public List<int> SwitchIds { get; set; }
        public List<int> DataSourceIds { get; set; }
        
        public DateTime? EffectiveDate { get; set; }
    }
}
