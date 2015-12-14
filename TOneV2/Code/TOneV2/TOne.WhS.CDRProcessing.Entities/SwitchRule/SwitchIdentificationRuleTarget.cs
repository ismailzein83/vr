using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SwitchIdentificationRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleSwitchDataSourceTarget
    {
        public int? DataSource { get; set; }
    }
}
