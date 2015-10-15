using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleAdjustNumberTarget : Vanrise.Rules.BaseRuleTargetIdentifier
    {
        public string PhoneNumber { get; set; }
    }
}
