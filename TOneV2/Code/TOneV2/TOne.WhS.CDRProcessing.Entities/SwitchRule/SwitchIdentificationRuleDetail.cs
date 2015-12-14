using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SwitchIdentificationRuleDetail
    {
        public SwitchIdentificationRule Entity { get; set; }
        public string SwitchName { get; set; }
        public string DataSourcesNames { get; set; }

    }
}
